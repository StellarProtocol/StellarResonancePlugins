using System.Collections.Generic;
using Stellar.Abstractions.Domain;

namespace Stellar.CombatMeter;

public sealed partial class Plugin
{
    private const int HistoryCapacity = 10;
    private readonly List<EncounterHistoryEntry> _history = new();
    private string? _lastSceneName;

    internal sealed class EncounterHistoryEntry
    {
        public string?  SceneName;
        public long     EnteredAtMs;
        public long     ArchivedAtMs;
        public long     CombatDurationMs;
        public Dictionary<EntityId, SourceStats> Stats = new();
        public PartyType PartyType;
        public int       MemberCount;
    }

    private void OnSceneChanged(string? newScene)
    {
        if (_lastSceneName is null)
        {
            _lastSceneName = newScene;
            return;
        }

        // Auto-archive on scene change. ManualArchive() is the single source of
        // truth for the snapshot-and-clear flow; the Archive button calls it too.
        ManualArchive();

        _lastSceneName = newScene;
    }

    // Snapshot the active _stats into history and reset the live meter. No-op
    // when there's nothing to archive so the button-press path doesn't push
    // empty entries. Used by:
    //   - OnSceneChanged (automatic, on scene transition)
    //   - DrawHeaderBar's Archive button (manual, user-driven)
    internal void ManualArchive()
    {
        if (_stats.Count == 0) return;

        var entry = new EncounterHistoryEntry
        {
            SceneName        = _lastSceneName,
            EnteredAtMs      = _combatStartMs,
            ArchivedAtMs     = _services.CombatSnapshot.ServerNowMs,
            CombatDurationMs = ComputeDurationMs(),
            Stats            = DeepCopyStats(),
            PartyType        = _services.PartySnapshot.PartyType,
            // Combatant count — every entity that participated, not just party.
            // Guarded by _stats.Count == 0 early-return above, so >= 1 here.
            MemberCount      = _stats.Count,
        };
        _history.Add(entry);
        if (_history.Count > HistoryCapacity) _history.RemoveAt(0);

        Clear();
    }

    private long ComputeDurationMs()
    {
        long earliest = long.MaxValue, latest = 0;
        foreach (var s in _stats.Values)
        {
            if (s.FirstHitMs > 0 && s.FirstHitMs < earliest) earliest = s.FirstHitMs;
            if (s.LastHitMs  > latest)                       latest   = s.LastHitMs;
        }
        return earliest == long.MaxValue ? 0 : latest - earliest;
    }

    private Dictionary<EntityId, SourceStats> DeepCopyStats()
    {
        var copy = new Dictionary<EntityId, SourceStats>(_stats.Count);
        foreach (var (id, src) in _stats)
        {
            var s2 = new SourceStats
            {
                TotalDamage  = src.TotalDamage,
                TotalHealing = src.TotalHealing,
                TopHit       = src.TopHit,
                Hits         = src.Hits,
                Crits        = src.Crits,
                Kills        = src.Kills,
                FirstHitMs   = src.FirstHitMs,
                LastHitMs    = src.LastHitMs,
                BySkill      = new Dictionary<int, SkillStats>(src.BySkill.Count),
            };
            foreach (var (sid, sk) in src.BySkill)
            {
                s2.BySkill[sid] = new SkillStats
                {
                    Total  = sk.Total,
                    Hits   = sk.Hits,
                    Crits  = sk.Crits,
                    TopHit = sk.TopHit,
                };
            }
            copy[id] = s2;
        }
        return copy;
    }
}
