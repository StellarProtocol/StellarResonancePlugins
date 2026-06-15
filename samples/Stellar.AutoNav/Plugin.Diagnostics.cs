using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Stellar.AutoNav;

public sealed partial class Plugin
{
    // -------------------------------------------------------------------------
    // Diagnostic helpers
    // -------------------------------------------------------------------------

    private void DumpButtons(string label)
    {
        try
        {
            // Resources.FindObjectsOfTypeAll catches inactive too; we filter by activeInHierarchy below.
            var all = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Button>());
            var active = new List<string>();
            for (var i = 0; i < all.Length; i++)
            {
                var raw = all[i];
                var btn = raw?.TryCast<Button>();
                if (btn == null || !btn.gameObject.activeInHierarchy) continue;

                var path = TransformPath(btn.transform);
                var text = TryGetText(btn);
                var handlers = DescribeHandlers(btn.gameObject);
                var listenerCount = 0;
                try { listenerCount = btn.onClick.GetPersistentEventCount(); } catch { }

                var labelPart = string.IsNullOrEmpty(text) ? path : $"{path}  text='{text}'";
                active.Add($"{labelPart}  handlers={handlers}; listeners={listenerCount}");
            }

            _services.Log.Info($"[AutoNav] === {label}: {active.Count} active Button(s) ===");
            foreach (var line in active)
            {
                _services.Log.Info($"[AutoNav]   {line}");
            }
            _services.Log.Info($"[AutoNav] === end {label} ===");
        }
        catch (Exception ex)
        {
            _services.Log.Warning($"[AutoNav] dump failed: {ex.Message}");
        }
    }

    private static string TransformPath(Transform t)
    {
        var parts = new List<string>();
        var current = t;
        while (current != null)
        {
            parts.Add(string.IsNullOrEmpty(current.gameObject.name) ? "?" : current.gameObject.name);
            current = current.parent;
        }
        parts.Reverse();
        return string.Join("/", parts);
    }

    private static string TryGetText(Button btn)
    {
        try
        {
            var t = btn.GetComponentInChildren<Text>();
            return t?.text ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    // Dump ALL component types on the button GameObject + its first 3 parents.
    // Earlier IxxxHandler-only filter found nothing on the Title button — the click
    // handler is via a non-standard mechanism. Cast a wider net and skip only the
    // boring Unity primitives so the next iteration can identify the game-specific
    // class to invoke via reflection.
    private static readonly HashSet<string> _boringComponentTypes = new()
    {
        "UnityEngine.RectTransform",
        "UnityEngine.Transform",
        "UnityEngine.CanvasRenderer",
        "UnityEngine.UI.Image",
        "UnityEngine.UI.RawImage",
        "UnityEngine.UI.Text",
        "UnityEngine.UI.Shadow",
        "UnityEngine.UI.Outline",
        "UnityEngine.UI.LayoutElement",
        "UnityEngine.UI.HorizontalLayoutGroup",
        "UnityEngine.UI.VerticalLayoutGroup",
        "UnityEngine.UI.GridLayoutGroup",
        "UnityEngine.UI.ContentSizeFitter",
        "UnityEngine.UI.AspectRatioFitter",
        "UnityEngine.UI.Mask",
        "UnityEngine.UI.RectMask2D",
        "UnityEngine.GameObject",
    };

    private static string DescribeHandlers(GameObject go)
    {
        if (go == null) return string.Empty;

        var lines = new List<string>();
        var current = go.transform;
        var depth = 0;
        const int MaxDepth = 4;  // self + 3 parents
        while (current != null && depth < MaxDepth)
        {
            var components = current.gameObject.GetComponents<Component>();
            var seen = new List<string>();
            for (var i = 0; i < components.Length; i++)
            {
                var c = components[i];
                if (c == null) continue;
                // Under Il2CppInterop, c.GetType() returns the base Component wrapper —
                // not what we want. Unity components' ToString() returns
                // "gameObjectName (RealTypeName)"; extract the parenthesised part.
                var typeName = ExtractRealType(c);
                if (_boringComponentTypes.Contains(typeName)) continue;
                if (typeName == "UnityEngine.UI.Button" && depth == 0) continue; // we already log Button info separately
                if (typeName.StartsWith("Il2Cpp", StringComparison.Ordinal))
                {
                    var dotIdx = typeName.IndexOf('.');
                    if (dotIdx > 0 && dotIdx < typeName.Length - 1) typeName = typeName.Substring(dotIdx + 1);
                }
                seen.Add(typeName);
            }
            if (seen.Count > 0)
            {
                var prefix = depth == 0 ? "self" : $"parent+{depth}";
                lines.Add($"{prefix}=[{string.Join(", ", seen)}]");
            }
            current = current.parent;
            depth++;
        }

        return lines.Count == 0 ? "(no non-boring components on self/parents)" : string.Join(" ; ", lines);
    }

    // Unity's Component.ToString() yields "<gameObject> (<RealType>)". The real
    // type lives in the last parenthesised segment. Falls back to managed type if
    // ToString doesn't match the Unity pattern.
    private static string ExtractRealType(Component c)
    {
        try
        {
            var s = c.ToString() ?? string.Empty;
            var open = s.LastIndexOf('(');
            var close = s.LastIndexOf(')');
            if (open >= 0 && close > open)
            {
                return s.Substring(open + 1, close - open - 1);
            }
        }
        catch { /* fall through */ }
        return c.GetType().FullName ?? c.GetType().Name;
    }
}
