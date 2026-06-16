using System;
using Stellar.Abstractions.Services;

namespace Stellar.CombatMeter;

/// <summary>
/// The source detail block shown above the per-skill list in the Skill Breakdown window — a two-row grid of
/// metric-appropriate aggregate stats (Total, rate, Crit%, Max, Hits, Kills, Uptime, and the "other" metric)
/// for the drilled source. Values come from the captured <see cref="Plugin.EncounterHistoryEntry"/> via
/// <see cref="Plugin.MetricValueOf"/> / <see cref="Plugin.ComputeUptime"/>, keyed off the metric captured at
/// open time so the block stays consistent with the rows below it.
/// </summary>
public sealed partial class Plugin
{
    private HudElement BuildSkillDetailBlock() => new ColumnElement(new HudElement[]
    {
        new RowElement(new HudElement[]
        {
            DetailStat(() => "TOTAL", () => SkillDetail(0)),
            DetailStat(() => DetailRateKey(), () => SkillDetail(1)),
            DetailStat(() => "CRIT %", () => SkillDetail(2)),
            DetailStat(() => "MAX", () => SkillDetail(3)),
        }, Gap: 10f),
        new RowElement(new HudElement[]
        {
            DetailStat(() => "HITS", () => SkillDetail(4)),
            DetailStat(() => "KILLS", () => SkillDetail(5)),
            DetailStat(() => "UPTIME", () => SkillDetail(6)),
            DetailStat(() => DetailOtherKey(), () => SkillDetail(7)),
        }, Gap: 10f),
    }, Gap: 4f);

    private HudElement DetailStat(Func<string> key, Func<string> value)
        => new CellElement(new ColumnElement(new HudElement[]
        {
            new TextElement(key, MutedCol),
            new TextElement(value, Emphasis: true),
        }, Gap: 0f), Weight: 1f);

    private string DetailRateKey() => _skillBreakdown is { } sb ? MetricRateLabel(sb.Metric) : "DPS";

    private string DetailOtherKey() => _skillBreakdown?.Metric == Metric.Hps ? "DMG" : "HEAL";

    // Index → formatted aggregate value for the drilled source under its captured metric.
    private string SkillDetail(int slot)
    {
        if (_skillBreakdown is not { } sb || !sb.Session.Stats.TryGetValue(sb.Source, out var s)) return "—";
        long dur = sb.Session.CombatDurationMs;
        long total = MetricValueOf(s, sb.Metric);
        float crit = s.Hits > 0 ? 100f * s.Crits / s.Hits : 0f;
        return slot switch
        {
            0 => FormatAmount(total),
            1 => FormatAmount(ComputeArchivedDps(total, dur)),
            2 => $"{crit:F0}%",
            3 => FormatAmount(s.TopHit),
            4 => s.Hits.ToString(),
            5 => s.Kills.ToString(),
            6 => $"{ComputeUptime(s.FirstHitMs, s.LastHitMs, dur) * 100f:F0}%",
            7 => sb.Metric == Metric.Hps ? FormatAmount(s.TotalDamage) : FormatAmount(s.TotalHealing),
            _ => "—",
        };
    }
}
