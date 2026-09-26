using System;
using System.Reflection;
using UnityEngine;

namespace Stellar.AutoNav;

public sealed partial class Plugin
{
    // -------------------------------------------------------------------------
    // ZButton reflection helpers
    // -------------------------------------------------------------------------

    // Resolves Panda.ZUi.ZButton.invokeClickEvent() once, then caches.
    private static Type? _zbuttonType;
    private static MethodInfo? _invokeClickEventMethod;

    private static bool TryResolveZButtonInvoker()
    {
        if (_invokeClickEventMethod != null) return true;

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type? t;
            try
            {
                t = asm.GetType("Panda.ZUi.ZButton", throwOnError: false);
            }
            catch
            {
                continue;
            }
            if (t == null) continue;
            _zbuttonType = t;
            _invokeClickEventMethod = t.GetMethod(
                "invokeClickEvent",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (_invokeClickEventMethod != null) return true;
        }
        return false;
    }

    // Find the Panda.ZUi.ZButton component on the GameObject by reflecting its real
    // type (Il2CppInterop wraps everything as Component, so we use ExtractRealType
    // to identify it, then return that wrapper — reflection's Invoke can still call
    // instance methods on it because under HybridCLR the receiving type is managed).
    private Component? FindZButton(GameObject go)
    {
        if (go == null) return null;
        var components = go.GetComponents<Component>();
        for (var i = 0; i < components.Length; i++)
        {
            var c = components[i];
            if (c == null) continue;
            if (ExtractRealType(c) == "Panda.ZUi.ZButton")
            {
                return c;
            }
        }
        return null;
    }

    private bool InvokeZButtonClick(string path, string label)
    {
        try
        {
            return EnsureZButtonInvoker(label) && TryInvokeReversePatchOnce(path, label);
        }
        catch (Exception ex)
        {
            _services.Log.Warning($"[AutoNav] CLICK FAILED ({label}): {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    /// <summary>Clicks the ZButton on an already-resolved transform (the polling steps find targets by name).</summary>
    private bool ClickZButtonOn(Transform target, string label)
    {
        try
        {
            return EnsureZButtonInvoker(label) && ClickResolved(target, label, TransformPath(target));
        }
        catch (Exception ex)
        {
            _services.Log.Warning($"[AutoNav] CLICK FAILED ({label}): {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    // Resolve the type + apply the reverse patch (cached/idempotent).
    private bool EnsureZButtonInvoker(string label)
    {
        if (!TryResolveZButtonInvoker())
            return WarnFail(label, "Panda.ZUi.ZButton or invokeClickEvent() not found in any loaded assembly");
        if (!ZButtonReversePatch.TryApply(_zbuttonType!, _services.Log))
            return WarnFail(label, "reverse patch could not be applied");
        return true;
    }

    // Must remain `private` in the same class as InvokeZButtonClick — HarmonyX
    // reverse-patch trampolines are scoped to the calling site.
    private bool TryInvokeReversePatchOnce(string path, string label)
    {
        var slash = path.IndexOf('/');
        if (slash < 0) return WarnFail(label, "path needs root + child");
        var root = GameObject.Find(path.Substring(0, slash));
        if (root == null) return WarnFail(label, "root not found");
        var target = root.transform.Find(path.Substring(slash + 1));
        if (target == null) return WarnFail(label, $"path '{path}' not found");
        return ClickResolved(target, label, path);
    }

    private bool ClickResolved(Transform target, string label, string path)
    {
        var zbutton = FindZButton(target.gameObject);
        if (zbutton == null) return WarnFail(label, $"no Panda.ZUi.ZButton on {path}");

        // Reverse patch redirects this call to ZButton.invokeClickEvent(instance).
        ZButtonReversePatch.ClickStub(zbutton);
        _services.Log.Info($"[AutoNav] CLICK '{label}' via reverse patch ({path})");
        return true;
    }

    private bool WarnFail(string label, string reason)
    {
        _services.Log.Warning($"[AutoNav] CLICK FAILED ({label}): {reason}");
        return false;
    }
}
