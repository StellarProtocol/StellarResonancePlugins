using System.Collections.Generic;
using Stellar.Abstractions.Domain;
using Stellar.Abstractions.Services;

namespace Stellar.CombatMeter;

/// <summary>
/// Projects the selected session's frozen per-source <see cref="SourceSeries"/> into <see cref="ChartSeries"/>
/// for the history timeline chart: always a muted team-total line (summed per bucket at build) plus one
/// role-coloured line per source toggled onto the chart via <see cref="ToggleChartSource"/>.
///
/// <para><b>Stable-ref caching:</b> <see cref="LineChartElement"/> bakes its Y-max / tick values from the
/// <c>Series</c> list it is handed, so the chart must receive the <em>same</em> <see cref="List{T}"/> instance
/// across frames where nothing changed (otherwise it churns/rebuilds every frame). <see cref="BuildChartSeries"/>
/// therefore returns a cached list and only repopulates it when a small dirty signature changes — the selected
/// session reference, the history metric, or a version int bumped by <see cref="ToggleChartSource"/>. The list
/// <em>instance</em> is reused; only its contents are rewritten, so element-side identity is preserved.</para>
/// </summary>
public sealed partial class Plugin
{
    private const int MaxChartSources = 8;     // top-N series captured per spec §5.2
    private readonly HashSet<EntityId> _chartedSources = new();   // rows toggled onto the chart
    private (float Min, float Max) _chartVisibleRange;

    // Muted near-white for the always-present team-total line (sits above the role-coloured source lines).
    private static readonly ColorRgba TeamTotalColor = new(0.88f, 0.90f, 0.92f, 1f);

    // Stable chart-series buffer + its dirty signature.
    private readonly List<ChartSeries> _chartSeries = new();
    private EncounterHistoryEntry? _chartSeriesSession;
    private Metric _chartSeriesMetric = (Metric)(-1);
    private int _chartSeriesBuiltVersion = -1;
    private int _chartSourcesVersion;             // bumped by ToggleChartSource

    private static long[] ChannelOf(SourceSeries s, Metric m) => m switch
    {
        Metric.Hps   => s.Healing,
        Metric.Taken => s.Taken,
        _            => s.Dealt,
    };

    internal static long[] TeamTotalSeries(EncounterHistoryEntry h, Metric m)
    {
        int len = 0;
        foreach (var s in h.Series.Values)
        {
            var ch = ChannelOf(s, m);
            if (ch.Length > len) len = ch.Length;
        }
        var total = new long[len];
        foreach (var s in h.Series.Values)
        {
            var ch = ChannelOf(s, m);
            for (int i = 0; i < ch.Length; i++) total[i] += ch[i];
        }
        return total;
    }

    // Build the ChartSeries set: team total (emphasis) + one per toggled source present in the session.
    // Returns a STABLE list instance — repopulated only when the dirty signature changes (see class summary).
    private IReadOnlyList<ChartSeries> BuildChartSeries()
    {
        if (!ChartSeriesDirty()) return _chartSeries;

        _chartSeries.Clear();
        _chartSeriesSession      = _selectedSession;
        _chartSeriesMetric       = _historyMetric;
        _chartSeriesBuiltVersion = _chartSourcesVersion;

        if (_selectedSession is not { } h) return _chartSeries;

        long teamTotalValue = ComputeSessionMetricTotal(h, _historyMetric);
        _chartSeries.Add(new ChartSeries(
            "Team total", TeamTotalColor,
            ToFloat(SeriesOrBucketZero(TeamTotalSeries(h, _historyMetric), teamTotalValue)),
            Emphasis: true));

        EntityId self = _services.CombatSnapshot.LocalEntityId;
        foreach (var id in _chartedSources)
            if (h.Series.TryGetValue(id, out var s))
            {
                long sourceTotal = h.Stats.TryGetValue(id, out var st) ? MetricValueOf(st, _historyMetric) : 0L;
                _chartSeries.Add(new ChartSeries(
                    EntityLabel.Resolve(id, self, _services.PlayerState, _services.CombatLookup, _services.PartyRoster.Members),
                    RoleColorFor(id),
                    ToFloat(SeriesOrBucketZero(ChannelOf(s, _historyMetric), sourceTotal))));
            }

        return _chartSeries;
    }

    private bool ChartSeriesDirty()
        => !ReferenceEquals(_chartSeriesSession, _selectedSession)
           || _chartSeriesMetric != _historyMetric
           || _chartSeriesBuiltVersion != _chartSourcesVersion;

    // Sub-bucket encounters never accumulate a bucketed timeline (the encounter ended before the first bucket
    // closed), so the per-bucket channel is empty and the chart would scan an all-zero/empty window → peak 0 →
    // the degenerate 1/0/0/0 Y axis. Fall back to a single bucket-0 point carrying the source's metric total so
    // the peak (and therefore the axis) reflects real data. Non-empty channels pass through unchanged.
    internal static long[] SeriesOrBucketZero(long[] channel, long fallbackTotal)
        => channel.Length > 0 ? channel : new[] { fallbackTotal };

    private float SeriesBucketSeconds(EncounterHistoryEntry h)
    {
        foreach (var s in h.Series.Values) return s.BucketMs / 1000f;
        return 1f;
    }

    private static IReadOnlyList<float> ToFloat(long[] a)
    {
        var f = new float[a.Length];
        for (int i = 0; i < a.Length; i++) f[i] = a[i];
        return f;
    }

    private void ToggleChartSource(EntityId id)
    {
        if (!_chartedSources.Remove(id)) _chartedSources.Add(id);
        _chartSourcesVersion++;   // mark the cached series stale
    }
}
