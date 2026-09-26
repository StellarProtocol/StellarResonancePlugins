using System;
using System.Reflection;
using HarmonyLib;
using Stellar.Abstractions.Services;

namespace Stellar.AutoNav;

/// <summary>
/// HarmonyX postfix pulse on <c>Panda.Core.Game.Update(float)</c>. The framework's
/// InvokeRepeating tick is gated off until logged-in + in-world (the scene-transition
/// gate in Wiring.GameLoop.cs), so plugin Update never fires on the Title/Char-Select
/// screens — exactly where AutoNav must click. This pulse restores the pre-gate drive
/// (the old framework used this same hook), for AutoNav only and only when
/// <c>STELLAR_AUTONAV=1</c>.
/// </summary>
internal static class GameUpdatePulse
{
    private static bool _patched;
    private static Harmony? _harmony;

    /// <summary>Invoked from the Game.Update postfix with the game's own deltaTime.</summary>
    internal static Action<float>? OnPulse;

    /// <summary>Applies the postfix. Idempotent. Returns true on success.</summary>
    public static bool TryApply(IPluginLog log)
    {
        if (_patched) return true;

        var gameType = FindType("Panda.Core.Game");
        if (gameType == null)
        {
            log.Warning("[AutoNav] pulse FAILED: Panda.Core.Game not loaded");
            return false;
        }

        MethodInfo? update = null;
        foreach (var m in gameType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (m.Name != "Update") continue;
            var ps = m.GetParameters();
            if (ps.Length == 1 && ps[0].ParameterType == typeof(float)) { update = m; break; }
        }
        if (update == null)
        {
            log.Warning("[AutoNav] pulse FAILED: Panda.Core.Game.Update(float) not found");
            return false;
        }

        try
        {
            _harmony = new Harmony("stellar.autonav.pulse");
            _harmony.Patch(update, postfix: new HarmonyMethod(typeof(GameUpdatePulse), nameof(Postfix)));
            _patched = true;
            log.Info("[AutoNav] pulse installed: Panda.Core.Game.Update postfix drives AutoNav scheduling (framework tick is login-gated)");
            return true;
        }
        catch (Exception ex)
        {
            log.Warning($"[AutoNav] pulse FAILED: {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    // HarmonyX binds __0 to the original's first argument (deltaTime).
    private static void Postfix(float __0)
    {
        try { OnPulse?.Invoke(__0); }
        catch { /* never leak exceptions into the game loop */ }
    }

    /// <summary>Undo on Dispose so a soft-cycle re-enable doesn't double-patch.</summary>
    public static void Unpatch()
    {
        if (!_patched || _harmony is null) return;
        try { _harmony.UnpatchSelf(); } catch { }
        _harmony = null;
        _patched = false;
        OnPulse = null;
    }

    private static Type? FindType(string fullName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            try { var t = asm.GetType(fullName, throwOnError: false); if (t != null) return t; }
            catch { }
        }
        return null;
    }
}

