using System.Collections.Generic;
using Stellar.Abstractions.Domain;

namespace Stellar.CombatMeter;

/// <summary>Deserialize half of <see cref="HistoryStore"/> — the reflection-free, never-throwing reader.</summary>
internal static partial class HistoryStore
{
    /// <summary>
    /// Parse one serialized entry. Returns false (and leaves <paramref name="entry"/> null) on ANY malformed or
    /// unsupported input — the caller skips it and keeps the rest of the history.
    /// </summary>
    internal static bool TryDeserializeEntry(string json, out Plugin.EncounterHistoryEntry? entry)
    {
        entry = null;
        var r = new HistoryJsonReader(json);
        if (r.Next() != JsonTokenKind.ObjectStart) return false;
        var e = new Plugin.EncounterHistoryEntry();
        if (!ReadEntryFields(r, e)) return false;
        // Reject anything that didn't carry the version marker (legacy / corrupt).
        entry = e;
        return true;
    }

    // Read the top-level entry object's key/value pairs. Returns false on structural error.
    private static bool ReadEntryFields(HistoryJsonReader r, Plugin.EncounterHistoryEntry e)
    {
        var seenVersion = false;
        while (true)
        {
            var k = r.Next();
            if (k == JsonTokenKind.ObjectEnd) return seenVersion;   // require "v" to have appeared
            if (k == JsonTokenKind.Comma) continue;
            if (k != JsonTokenKind.String) return false;            // key must be a string
            var key = r.StringValue;
            if (r.Next() != JsonTokenKind.Colon) return false;
            switch (key)
            {
                case "v":       if (!ReadLong(r, out var v)) return false; seenVersion = v == FormatVersion; break;
                case "scene":   if (!ReadString(r, out var sc)) return false; e.SceneName = sc; break;
                case "enter":   if (!ReadLong(r, out e.EnteredAtMs)) return false; break;
                case "arch":    if (!ReadLong(r, out e.ArchivedAtMs)) return false; break;
                case "dur":     if (!ReadLong(r, out e.CombatDurationMs)) return false; break;
                case "party":   if (!ReadLong(r, out var pt)) return false; e.PartyType = (PartyType)(int)pt; break;
                case "members": if (!ReadLong(r, out var mc)) return false; e.MemberCount = (int)mc; break;
                case "stats":   if (!ReadStats(r, e.Stats)) return false; break;
                case "series":  if (!ReadSeries(r, e.Series)) return false; break;
                default: return false;
            }
        }
    }

    private static bool ReadStats(HistoryJsonReader r, Dictionary<EntityId, SourceStats> into)
        => ReadArray(r, () =>
        {
            var s = new SourceStats();
            long id = 0;
            if (!ReadObject(r, key =>
            {
                switch (key)
                {
                    case "id":  return ReadLong(r, out id);
                    case "td":  return ReadLong(r, out s.TotalDamage);
                    case "th":  return ReadLong(r, out s.TotalHealing);
                    case "tk":  return ReadLong(r, out s.TotalTaken);
                    case "top": return ReadLong(r, out s.TopHit);
                    case "h":   return ReadInt(r, out s.Hits);
                    case "c":   return ReadInt(r, out s.Crits);
                    case "k":   return ReadInt(r, out s.Kills);
                    case "fh":  return ReadLong(r, out s.FirstHitMs);
                    case "lh":  return ReadLong(r, out s.LastHitMs);
                    case "sk":  return ReadSkills(r, s.BySkill);
                    case "in":  return ReadIncoming(r, s.IncomingBySkill);
                    default:    return false;
                }
            })) return false;
            into[new EntityId(id)] = s;
            return true;
        });

    private static bool ReadSkills(HistoryJsonReader r, Dictionary<int, SkillStats> into)
        => ReadArray(r, () =>
        {
            var sk = new SkillStats(); int id = 0;
            if (!ReadObject(r, key => key switch
            {
                "id"  => ReadInt(r, out id),
                "t"   => ReadLong(r, out sk.Total),
                "ht"  => ReadLong(r, out sk.HealTotal),
                "h"   => ReadInt(r, out sk.Hits),
                "c"   => ReadInt(r, out sk.Crits),
                "top" => ReadLong(r, out sk.TopHit),
                _     => false,
            })) return false;
            into[id] = sk;
            return true;
        });

    private static bool ReadIncoming(HistoryJsonReader r, Dictionary<int, IncomingSkillStats> into)
        => ReadArray(r, () =>
        {
            var inc = new IncomingSkillStats(); int id = 0;
            if (!ReadObject(r, key => key switch
            {
                "id"  => ReadInt(r, out id),
                "t"   => ReadLong(r, out inc.Total),
                "h"   => ReadInt(r, out inc.Hits),
                "top" => ReadLong(r, out inc.TopHit),
                _     => false,
            })) return false;
            into[id] = inc;
            return true;
        });

    private static bool ReadSeries(HistoryJsonReader r, Dictionary<EntityId, SourceSeries> into)
        => ReadArray(r, () =>
        {
            var sr = new SourceSeries(); long id = 0;
            if (!ReadObject(r, key =>
            {
                switch (key)
                {
                    case "id": return ReadLong(r, out id);
                    case "b":  return ReadInt(r, out sr.BucketMs);
                    case "d":  return ReadLongArray(r, out sr.Dealt);
                    case "hl": return ReadLongArray(r, out sr.Healing);
                    case "tk": return ReadLongArray(r, out sr.Taken);
                    default:   return false;
                }
            })) return false;
            into[new EntityId(id)] = sr;
            return true;
        });
}
