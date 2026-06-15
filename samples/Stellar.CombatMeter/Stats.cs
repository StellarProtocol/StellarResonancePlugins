using System.Collections.Generic;

namespace Stellar.CombatMeter;

/// <summary>
/// Per-source aggregate. Mutated only on the Unity main thread from
/// <see cref="Plugin.OnCombatEvent"/>; read from the same thread during
/// <see cref="Plugin.OnDraw"/>. No locks required.
/// </summary>
internal sealed class SourceStats
{
    public long TotalDamage;
    public long TotalHealing;
    public long TopHit;
    public int  Hits;
    public int  Crits;
    public int  Kills;
    public long FirstHitMs;
    public long LastHitMs;
    public Dictionary<int, SkillStats> BySkill = new();
}

/// <summary>Per-skill aggregate inside a <see cref="SourceStats"/> entry.</summary>
internal sealed class SkillStats
{
    public long Total;
    public int  Hits;
    public int  Crits;
    public long TopHit;
}
