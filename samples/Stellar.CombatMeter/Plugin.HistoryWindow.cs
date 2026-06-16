using System;
using System.Collections.Generic;
using Stellar.Abstractions.Domain;
using Stellar.Abstractions.Services;

namespace Stellar.CombatMeter;

/// <summary>
/// History surface — a uGUI master-detail window (Party chrome). Left pane: a scrollable session list (newest
/// first, SelectableElement rows). Right pane: the selected session's per-source table (rank·name·class +
/// TOTAL DMG / DPS / %DMG columns) with a drill-in ► button that fires <see cref="OnSkillBreakdownRequested"/>.
/// The plugin snapshots both panes into _historyView / _sessionRows each shown frame.
/// </summary>
public sealed partial class Plugin
{
    internal event Action<EntityId, EncounterHistoryEntry>? OnSkillBreakdownRequested;

    private const int MaxSessionSlots = HistoryCapacity;   // session list bound
    private const int MaxSourceSlots  = 24;                 // detail rows bound
    private const float HistListHeight   = 300f;
    private const float HistDetailHeight = 260f;
    // Detail table columns (right-aligned numerics) — TOTAL DMG · DPS · HPS · COUNT · HEAL · %DMG + drill ►.
    private const float ColDmg = 72f, ColDps = 56f, ColHps = 56f, ColCount = 46f, ColHeal = 56f, ColPct = 48f, ColDrill = 26f;

    private int _historyIndex = -1;   // -1 = no session selected (original history-list index)
    private EncounterHistoryEntry? _selectedSession;

    private readonly List<SessionEntry> _historyView = new(MaxSessionSlots);
    private readonly List<SourceRow> _sessionRows = new(MaxSourceSlots);

    private readonly struct SessionEntry
    {
        public SessionEntry(int idx, string time, string dur, string players, string scene)
        { Index = idx; Time = time; Dur = dur; Players = players; Scene = scene; }
        public readonly int Index; public readonly string Time, Dur, Players, Scene;
    }

    // Field struct (no constructor) — keeps clear of the analyzer's ctor-dependency cap.
    private struct SourceRow
    {
        public EntityId Id;
        public string Rank, Name, Class, Dmg, Dps, Hps, Count, Heal, Pct;
        public float Share;
        public ColorRgba Role;
    }

    private HudElement BuildHistoryRoot() => new RowElement(new HudElement[]
    {
        new CellElement(BuildSessionList(), Weight: 1f),
        new SeparatorElement(Vertical: true),
        new CellElement(BuildSessionDetail(), Weight: 2f),
    }, Gap: 8f);

    private HudElement BuildSessionList()
    {
        var slots = new HudElement[MaxSessionSlots];
        for (var i = 0; i < MaxSessionSlots; i++)
        {
            var idx = i;
            slots[i] = new SelectableElement(
                new ColumnElement(new HudElement[]
                {
                    new TextElement(() => idx < _historyView.Count ? "⏱ " + _historyView[idx].Time : "", Emphasis: true),
                    new TextElement(() => idx < _historyView.Count ? _historyView[idx].Dur : "", MutedCol),
                    new TextElement(() => idx < _historyView.Count ? _historyView[idx].Players : "", MutedCol),
                    new TextElement(() => idx < _historyView.Count ? _historyView[idx].Scene : "", MutedCol),
                }, Gap: 1f),
                OnClick: () => { if (idx < _historyView.Count) SelectSession(_historyView[idx].Index); },
                Selected: () => idx < _historyView.Count && _historyView[idx].Index == _historyIndex);
        }
        return new ColumnElement(new HudElement[]
        {
            new ConditionalElement(() => _history.Count == 0,
                new TextElement(() => "No archived encounters yet.", MutedCol)),
            new ConditionalElement(() => _history.Count > 0,
                new ScrollElement(new ListElement(() => _historyView.Count, slots), HistListHeight)),
        });
    }

    private HudElement BuildSessionDetail()
    {
        var slots = new HudElement[MaxSourceSlots];
        for (var i = 0; i < MaxSourceSlots; i++) slots[i] = BuildSourceRowSlot(i);
        var table = new ColumnElement(new HudElement[]
        {
            BuildHistoryMetricRow(),
            new TextElement(SessionSummary, Emphasis: true),
            BuildHistoryChart(),
            BuildDetailHeaderRow(),
            new ScrollElement(new ListElement(() => _sessionRows.Count, slots), HistDetailHeight),
        });
        return new ColumnElement(new HudElement[]
        {
            new ConditionalElement(() => _selectedSession is null,
                new TextElement(() => "Pick a session on the left.", MutedCol)),
            new ConditionalElement(() => _selectedSession is not null, table),
        });
    }

    // The timeline chart: team-total (always) + a line per source toggled into _chartedSources. Axis scale +
    // Y title follow _historyMetric; rebuilt (not refreshed) on metric change so the baked axis rescales.
    private HudElement BuildHistoryChart() => new LineChartElement(
        Series:          BuildChartSeries,
        BucketSeconds:   () => _selectedSession is { } h ? SeriesBucketSeconds(h) : 1f,
        FormatY:         v => FormatAmount((long)v),
        FormatX:         FormatSeconds,
        TitleY:          () => MetricAxisTitle(_historyMetric),
        TitleX:          () => "Encounter time (m:ss)",
        VisibleRange:    () => _chartVisibleRange,
        SetVisibleRange: r => _chartVisibleRange = r,
        Width:           500f,
        Height:          180f);

    // Metric-aware column header — the primary value column + rate column relabel with _historyMetric.
    private HudElement BuildDetailHeaderRow() => new RowElement(new HudElement[]
    {
        new CellElement(new TextElement(() => "Source", MutedCol), Weight: 1f),
        NumCell(() => MetricColumnLabel(_historyMetric), ColDmg, muted: true),
        NumCell(() => MetricRateLabel(_historyMetric), ColDps, muted: true),
        NumCell(() => "HPS", ColHps, muted: true), NumCell(() => "COUNT", ColCount, muted: true),
        NumCell(() => "HEAL", ColHeal, muted: true), NumCell(() => "%", ColPct, muted: true),
        new CellElement(new TextElement(() => ""), Width: ColDrill),
    }, Gap: 6f);

    // One source row: AccentRowElement (metric-share stripe) wrapped in a SelectableElement so a body click
    // toggles the chart line, while the inner ► ButtonElement keeps its own hit area for the drill-in.
    private HudElement BuildSourceRowSlot(int i)
    {
        var idx = i;
        string F(Func<SourceRow, string> sel) => idx < _sessionRows.Count ? sel(_sessionRows[idx]) : "";
        var row = new RowElement(new HudElement[]
        {
            new CellElement(new ColumnElement(new HudElement[]
            {
                new TextElement(() => idx < _sessionRows.Count ? $"{_sessionRows[idx].Rank} {_sessionRows[idx].Name}" : "", Emphasis: true),
                new TextElement(() => F(r => r.Class), MutedCol),
            }, Gap: 0f), Weight: 1f),
            NumCell(() => F(r => r.Dmg), ColDmg),
            NumCell(() => F(r => r.Dps), ColDps),
            NumCell(() => F(r => r.Hps), ColHps),
            NumCell(() => F(r => r.Count), ColCount),
            NumCell(() => F(r => r.Heal), ColHeal),
            NumCell(() => F(r => r.Pct), ColPct),
            new CellElement(new ButtonElement(() => DrillLabel(idx), () => DrillIn(idx),
                Active: () => DrillOpen(idx)), Width: ColDrill),
        }, Gap: 6f);
        var accent = new AccentRowElement(row,
            () => idx < _sessionRows.Count ? _sessionRows[idx].Role : default,
            () => idx < _sessionRows.Count ? _sessionRows[idx].Share : 0f);
        return new SelectableElement(accent,
            OnClick:  () => { if (idx < _sessionRows.Count) ToggleChartSource(_sessionRows[idx].Id); },
            Selected: () => idx < _sessionRows.Count && _chartedSources.Contains(_sessionRows[idx].Id));
    }

    // Right-aligned fixed-width numeric column cell.
    private HudElement NumCell(Func<string> text, float width, bool muted = false)
        => new CellElement(new TextElement(text, muted ? MutedCol : (Func<ColorRgba?>?)null, Align: TextAlign.Right), Width: width);

    private static string FormatSeconds(float s)
    {
        var total = (int)(s < 0f ? 0f : s);
        return $"{total / 60}:{total % 60:00}";
    }

    private string SessionSummary()
    {
        if (_selectedSession is not { } h) return "";
        return $"Combat Session  ·  {FormatSessionTimestampLong(h.ArchivedAtMs)}  ·  {FormatSessionDurationShort(h.CombatDurationMs)}  ·  {h.MemberCount}p";
    }

    private void SelectSession(int historyIndex)
    {
        _historyIndex = historyIndex;
        _selectedSession = historyIndex >= 0 && historyIndex < _history.Count ? _history[historyIndex] : null;
        // A new session => no carried-over chart lines, and the visible (zoom) window resets to the full span.
        _chartedSources.Clear();
        _chartSourcesVersion++;   // mark the cached chart series stale
        var durationSeconds = _selectedSession is { } h ? h.CombatDurationMs / 1000f : 0f;
        _chartVisibleRange = (0f, durationSeconds);
        RebuildSessionRows();
    }

    // A popup dialog (opened from the ≡ menu): free-drag + ✕ close. EditModeDragOnly defaults false, so it
    // drags freely (not editor-managed) even though it wears the Party chrome. Shared by the initial
    // registration (Plugin.cs) and the metric-change rebuild below.
    private IWindowControl RegisterHistoryWindow() => _services.Windows.Register(new WindowRegistration(
        new WindowSpec(
            Id:          "combatmeter.history",
            Title:       "Combat History",
            DefaultRect: new WindowRect(900f, 380f, 780f, 0f),
            Category:    WindowCategory.HUD,
            Style:       WindowPanelStyle.Party)
        { StartVisible = false, HideUntilInWorld = true, Closable = true, Draggable = true },
        BuildHistoryRoot(),
        OnClose: CloseHistory));

    // The LineChartElement bakes axis ticks at build time; rebuild the window subtree (preserving rect +
    // visibility) so a metric change rescales the Y axis. Framework-sanctioned Remove()+Register() pattern.
    private void RebuildHistoryWindow()
    {
        var rect = _historyWindow.Rect;
        var wasShown = _historyWindow.IsShown;
        _historyWindow.Remove();
        _historyWindow = RegisterHistoryWindow();
        // Position actually survives via the Draggable window's persisted LayoutStorage rect (restored on the
        // next mount); this SetRect is belt-and-suspenders (a no-op while Token is null pre-Tick) and the
        // explicit guard matters only if this window ever becomes non-Draggable/non-persisted.
        if (rect.Width > 0f) _historyWindow.SetRect(rect);
        _historyWindow.SetVisible(wasShown);
    }

    // ----- drill-in (►) -----

    private string DrillLabel(int idx) => DrillOpen(idx) ? "◄" : "►";

    private bool DrillOpen(int idx)
        => idx < _sessionRows.Count && _selectedSession is { } h && _skillBreakdown is { } sb
           && sb.Source == _sessionRows[idx].Id && ReferenceEquals(sb.Session, h);

    private void DrillIn(int idx)
    {
        if (idx >= _sessionRows.Count || _selectedSession is not { } h) return;
        OnSkillBreakdownRequested?.Invoke(_sessionRows[idx].Id, h);
    }

    // ----- snapshots -----

    private void RebuildHistorySnapshots()
    {
        _historyView.Clear();
        for (int i = _history.Count - 1; i >= 0; i--)   // newest first
        {
            var h = _history[i];
            _historyView.Add(new SessionEntry(
                i,
                FormatSessionTimestamp(h.ArchivedAtMs),
                FormatSessionDurationShort(h.CombatDurationMs),
                $"{h.MemberCount} players",
                h.SceneName ?? ""));
        }
        // Keep the selected session in sync (it may have been evicted).
        if (_historyIndex >= 0 && _historyIndex < _history.Count) _selectedSession = _history[_historyIndex];
        else { _selectedSession = null; _historyIndex = -1; _chartedSources.Clear(); _chartSourcesVersion++; }
        RebuildSessionRows();
    }

    private void RebuildSessionRows()
    {
        _sessionRows.Clear();
        if (_selectedSession is not { } h || h.Stats.Count == 0) return;

        var metric = _historyMetric;
        long metricTotal = ComputeSessionMetricTotal(h, metric);
        var rows = new List<KeyValuePair<EntityId, SourceStats>>(h.Stats.Count);
        foreach (var kv in h.Stats) rows.Add(kv);
        rows.Sort((a, b) => MetricValueOf(b.Value, metric).CompareTo(MetricValueOf(a.Value, metric)));

        EntityId self = _services.CombatSnapshot.LocalEntityId;
        for (int i = 0; i < rows.Count && _sessionRows.Count < MaxSourceSlots; i++)
        {
            var id = rows[i].Key; var s = rows[i].Value;
            long value = MetricValueOf(s, metric);
            var pct = metricTotal > 0 ? (float)value / metricTotal : 0f;
            _sessionRows.Add(new SourceRow
            {
                Id = id,
                Rank = $"#{i + 1}",
                Name = EntityLabel.Resolve(id, self, _services.PlayerState, _services.CombatLookup, _services.PartyRoster.Members),
                Class = GetClassLine(id),
                Dmg = FormatAmount(value),                                              // primary = metric value
                Dps = FormatAmount(ComputeArchivedDps(value, h.CombatDurationMs)),       // rate = metric / sec
                Hps = FormatAmount(ComputeArchivedDps(s.TotalHealing, h.CombatDurationMs)),
                Count = s.Hits.ToString(),
                Heal = FormatAmount(s.TotalHealing),
                Pct = FormatPercent(pct),
                Share = pct,
                Role = RoleColorFor(id),
            });
        }
    }

    // ----- pure formatting helpers (carried over from the IMGUI build) -----

    private static long ComputeArchivedDps(long totalDamage, long durationMs)
    {
        long secs = durationMs / 1000; if (secs < 1) secs = 1; return totalDamage / secs;
    }

    private static string FormatSessionTimestamp(long ms)
        => ms <= 0 ? "—" : DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime.ToString("M/d, h:mm tt");

    private static string FormatSessionTimestampLong(long ms)
        => ms <= 0 ? "—" : DateTimeOffset.FromUnixTimeMilliseconds(ms).LocalDateTime.ToString("M/d/yyyy, h:mm:ss tt");

    private static string FormatSessionDurationShort(long durationMs)
    {
        long secs = durationMs / 1000; if (secs < 0) secs = 0;
        long m = secs / 60, s = secs % 60;
        return m > 0 ? $"{m}m {s}s" : $"{s}s";
    }

    private static long ComputeSessionMetricTotal(EncounterHistoryEntry h, Metric m)
    {
        long sum = 0;
        foreach (var s in h.Stats.Values) sum += MetricValueOf(s, m);
        return sum;
    }

    private static string FormatPercent(float fraction) => $"{fraction * 100f:F1}%";
}
