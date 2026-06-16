using System;
using System.Collections.Generic;
using Stellar.Abstractions.Domain;
using Stellar.Abstractions.Services;

namespace Stellar.CombatMeter;

/// <summary>
/// Skill breakdown surface — a uGUI window (Party chrome) opened by the History drill-in ► button. Operates on
/// the <see cref="EncounterHistoryEntry"/> captured at click-time (decoupled from live state). Header (name —
/// Skill Breakdown + meta), a column header, and a scrollable per-skill table (rank·name·sub + TOTAL/DPS/COUNT).
/// The plugin snapshots the skill rows into _skillRows each shown frame.
/// </summary>
public sealed partial class Plugin
{
    private const int MaxSkillSlots = 40;
    private const float SkillScrollHeight = 280f;
    private const float SkillColTotal = 90f, SkillColDps = 70f, SkillColCount = 56f;

    private SkillBreakdownState? _skillBreakdown;
    private readonly List<SkillRow> _skillRows = new(MaxSkillSlots);

    private sealed class SkillBreakdownState
    {
        public EntityId Source;
        public EncounterHistoryEntry Session = null!;
    }

    private struct SkillRow
    {
        public string Rank, Name, Sub, Total, Dps, Count;
        public float Share;
    }

    private HudElement BuildSkillBreakdownRoot()
    {
        var slots = new HudElement[MaxSkillSlots];
        for (var i = 0; i < MaxSkillSlots; i++)
        {
            var idx = i;
            string F(Func<SkillRow, string> sel) => idx < _skillRows.Count ? sel(_skillRows[idx]) : "";
            var row = new RowElement(new HudElement[]
            {
                new CellElement(new ColumnElement(new HudElement[]
                {
                    new TextElement(() => idx < _skillRows.Count ? $"{_skillRows[idx].Rank} {_skillRows[idx].Name}" : "", Emphasis: true),
                    new TextElement(() => F(r => r.Sub), MutedCol),
                }, Gap: 0f), Weight: 1f),
                NumCell(() => F(r => r.Total), SkillColTotal),
                NumCell(() => F(r => r.Dps), SkillColDps),
                NumCell(() => F(r => r.Count), SkillColCount),
            }, Gap: 6f);
            // Skill rows share the source's role colour stripe; the wash tracks each skill's share of the source.
            slots[i] = new AccentRowElement(row,
                () => _skillBreakdown is { } sb ? RoleColorFor(sb.Source) : default,
                () => idx < _skillRows.Count ? _skillRows[idx].Share : 0f);
        }
        return new ColumnElement(new HudElement[]
        {
            new TextElement(SkillHeader, Emphasis: true),
            new TextElement(SkillMeta, MutedCol),
            new SeparatorElement(),
            new RowElement(new HudElement[]
            {
                new CellElement(new TextElement(() => "Skill", MutedCol), Weight: 1f),
                NumCell(() => "TOTAL DMG", SkillColTotal, muted: true),
                NumCell(() => "DPS", SkillColDps, muted: true),
                NumCell(() => "COUNT", SkillColCount, muted: true),
            }, Gap: 6f),
            new ConditionalElement(() => _skillRows.Count == 0,
                new TextElement(() => "No skills recorded for this source.", MutedCol)),
            new ConditionalElement(() => _skillRows.Count > 0,
                new ScrollElement(new ListElement(() => _skillRows.Count, slots), SkillScrollHeight)),
        });
    }

    private string SkillHeader()
    {
        if (_skillBreakdown is not { } sb) return "Skill Breakdown";
        EntityId self = _services.CombatSnapshot.LocalEntityId;
        var name = EntityLabel.Resolve(sb.Source, self, _services.PlayerState, _services.CombatLookup, _services.PartyRoster.Members);
        return $"{name} — Skill Breakdown";
    }

    private string SkillMeta()
    {
        if (_skillBreakdown is not { } sb) return "";
        var s = sb.Session;
        return $"{FormatSessionTimestampLong(s.ArchivedAtMs)}  ·  {FormatSessionDurationShort(s.CombatDurationMs)}  ·  {s.MemberCount}p";
    }

    private void RebuildSkillRows()
    {
        _skillRows.Clear();
        if (_skillBreakdown is not { } state) return;

        // Stale-session guard: history capacity is 10; close if the drilled entry was evicted.
        if (!_history.Contains(state.Session)) { CloseSkillBreakdown(); return; }
        if (!state.Session.Stats.TryGetValue(state.Source, out var src) || src.BySkill.Count == 0) return;

        var rows = new List<KeyValuePair<int, SkillStats>>(src.BySkill.Count);
        foreach (var kv in src.BySkill) rows.Add(kv);
        rows.Sort(static (a, b) => b.Value.Total.CompareTo(a.Value.Total));

        long sourceTotal = ComputeSourceSkillTotal(src);
        long durationMs = state.Session.CombatDurationMs;
        for (int i = 0; i < rows.Count && _skillRows.Count < MaxSkillSlots; i++)
        {
            var sk = rows[i].Value;
            float pctDmg = sourceTotal > 0 ? 100f * sk.Total / sourceTotal : 0f;
            float critPct = sk.Hits > 0 ? 100f * sk.Crits / sk.Hits : 0f;
            _skillRows.Add(new SkillRow
            {
                Rank = $"#{i + 1}",
                Name = ResolveSkillName(rows[i].Key),
                Sub = $"% DMG {pctDmg:F1}%   Count {sk.Hits}   Crit {critPct:F1}%",
                Total = FormatAmount(sk.Total),
                Dps = FormatAmount(ComputeArchivedDps(sk.Total, durationMs)),
                Count = sk.Hits.ToString(),
                Share = sourceTotal > 0 ? (float)sk.Total / sourceTotal : 0f,
            });
        }
    }

    private string ResolveSkillName(int skillId)
    {
        var info = _services.GameData.Combat.GetSkill(skillId);
        return info is { Name: { Length: > 0 } name } ? name : $"#{skillId}";
    }

    private static long ComputeSourceSkillTotal(SourceStats stats)
    {
        long sum = 0;
        foreach (var sk in stats.BySkill.Values) sum += sk.Total;
        return sum;
    }

    // Metric-aware per-skill value: HPS reads the heal total, everything else (DPS/Taken) the damage total.
    internal static long SkillMetricTotal(SkillStats sk, Metric m)
        => m == Metric.Hps ? sk.HealTotal : sk.Total;

    /// <summary>One incoming (Taken-mode) row: damage to the drilled source grouped by the attacker's skill.</summary>
    internal readonly record struct IncomingRow(int SkillId, long Total, int Hits, long TopHit);

    // Project a source's IncomingBySkill map into rows sorted by total taken (desc).
    internal static IReadOnlyList<IncomingRow> BuildIncomingRows(SourceStats src)
    {
        var rows = new List<IncomingRow>(src.IncomingBySkill.Count);
        foreach (var (sid, inc) in src.IncomingBySkill) rows.Add(new IncomingRow(sid, inc.Total, inc.Hits, inc.TopHit));
        rows.Sort(static (a, b) => b.Total.CompareTo(a.Total));
        return rows;
    }

    private void HandleSkillBreakdownRequested(EntityId id, EncounterHistoryEntry session)
    {
        if (_skillBreakdown is { } sb && sb.Source == id && ReferenceEquals(sb.Session, session))
        {
            CloseSkillBreakdown();
            return;
        }
        _skillBreakdown = new SkillBreakdownState { Source = id, Session = session };
        RebuildSkillRows();
        _skillBreakdownWindow.SetVisible(true);
    }

    private void CloseSkillBreakdown()
    {
        _skillBreakdown = null;
        _skillBreakdownWindow.SetVisible(false);
    }
}
