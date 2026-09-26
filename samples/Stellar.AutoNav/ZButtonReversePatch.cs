using System;
using System.Reflection;
using HarmonyLib;
using Stellar.Abstractions.Services;

namespace Stellar.AutoNav;

/// <summary>
/// HarmonyX reverse-patch holder. The body of <see cref="ClickStub"/> is replaced
/// at runtime with a direct call to Panda.ZUi.ZButton.invokeClickEvent on the
/// supplied instance — bypassing reflection's strict type check that rejected our
/// previous attempt at MethodInfo.Invoke(zbutton, null).
/// </summary>
internal static class ZButtonReversePatch
{
    private static bool _patched;
    private static Harmony? _harmony;

    /// <summary>
    /// Patched-at-runtime stub. Calling this invokes ZButton.invokeClickEvent()
    /// on the supplied instance.
    /// </summary>
    public static void ClickStub(object instance)
    {
        // The body is replaced by HarmonyX at runtime; this throw protects against
        // calling before the patch is applied.
        throw new InvalidOperationException("ClickStub called before HarmonyX reverse patch was applied");
    }

    /// <summary>Applies the reverse patch. Idempotent. Returns true on success.</summary>
    public static bool TryApply(Type zbuttonType, IPluginLog log)
    {
        if (_patched) return true;

        var original = zbuttonType.GetMethod(
            "invokeClickEvent",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (original == null)
        {
            log.Warning("[AutoNav] reverse patch FAILED: Panda.ZUi.ZButton.invokeClickEvent method not found");
            return false;
        }

        var stub = typeof(ZButtonReversePatch).GetMethod(
            nameof(ClickStub),
            BindingFlags.Public | BindingFlags.Static);
        if (stub == null)
        {
            log.Warning("[AutoNav] reverse patch FAILED: ClickStub method not found (internal error)");
            return false;
        }

        try
        {
            _harmony = new Harmony("stellar.autonav");
            _harmony.CreateReversePatcher(original, new HarmonyMethod(stub)).Patch();
            _patched = true;
            log.Info("[AutoNav] reverse patch applied: ZButton.invokeClickEvent ↔ ZButtonReversePatch.ClickStub");
            return true;
        }
        catch (Exception ex)
        {
            log.Warning($"[AutoNav] reverse patch FAILED: {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Reverse the patch on plugin Dispose so a soft-cycle re-enable doesn't
    /// double-patch (HarmonyX's CreateReversePatcher throws on duplicate
    /// registration). Idempotent — second call is a no-op.
    /// </summary>
    public static void Unpatch()
    {
        if (!_patched || _harmony is null) return;
        try { _harmony.UnpatchSelf(); }
        catch { /* swallow; nothing useful to log here */ }
        _harmony = null;
        _patched = false;
    }
}

