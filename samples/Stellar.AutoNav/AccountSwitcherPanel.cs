using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Stellar.AutoNav;

/// <summary>
/// Read-only view of the AccountSwitcher plugin's login panel as the framework builds it in uGUI:
/// <c>StellarWindowCanvas/accountswitcher.main/…/List/Slot/Row/Cell/Button</c> (the window root GameObject is
/// named after the window id; ListElement children are named <c>Slot</c>; each saved account is one Slot whose
/// Row holds the label Text, the status Text and a Button labelled <c>Login</c>). AutoNav never touches
/// AccountSwitcher itself — it presses the same button a player would.
/// </summary>
internal static class AccountSwitcherPanel
{
    public const string AssemblyName = "Stellar.AccountSwitcher";
    private const string WindowRootName = "accountswitcher.main";
    private const string CanvasName = "StellarWindowCanvas";
    private const string SlotName = "Slot";
    private const string LoginLabel = "Login";

    /// <summary><see cref="ReadStatusKind"/> value for a successful switch.</summary>
    public const string StatusSwitched = "switched";

    /// <summary>One visible saved-account row, in display order.</summary>
    internal readonly struct Row
    {
        public Row(int order, string label, Button login) { Order = order; Label = label; Login = login; }
        public int Order { get; }
        public string Label { get; }
        public Button Login { get; }
    }

    /// <summary>True when the AccountSwitcher plugin assembly is loaded in this process.</summary>
    public static bool IsLoaded()
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            try { if (asm.GetName().Name == AssemblyName) return true; }
            catch { /* dynamic/collectible assembly without a name — skip */ }
        }
        return false;
    }

    /// <summary>Active Login rows of the shown panel, sorted by display order (empty while the panel is
    /// not mounted/visible — ShouldRender gates it on the login phase).</summary>
    public static List<Row> FindLoginRows()
    {
        var rows = new List<Row>();
        var all = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Button>());
        for (var i = 0; i < all.Length; i++)
        {
            var btn = all[i]?.TryCast<Button>();
            if (btn == null || !btn.gameObject.activeInHierarchy || !btn.interactable) continue;
            if (ButtonLabel(btn) != LoginLabel) continue;
            var slot = FindSlotUnderPanel(btn.transform);
            if (slot == null) continue;
            rows.Add(new Row(slot.GetSiblingIndex(), SlotLabel(slot), btn));
        }
        rows.Sort((a, b) => a.Order.CompareTo(b.Order));
        return rows;
    }

    /// <summary>Classifies the panel's status line after a Login click: <c>switched</c>,
    /// <c>switch failed: &lt;lua error&gt;</c>, <c>slot expired</c>, or null when no such line is showing. The
    /// account label that AccountSwitcher embeds in the line is deliberately never returned.</summary>
    public static string? ReadStatusKind()
    {
        var all = Resources.FindObjectsOfTypeAll(Il2CppType.Of<Text>());
        for (var i = 0; i < all.Length; i++)
        {
            var t = all[i]?.TryCast<Text>();
            if (t == null || !t.gameObject.activeInHierarchy || !IsUnderPanel(t.transform)) continue;
            var s = t.text ?? string.Empty;
            if (s.StartsWith("switched to ", StringComparison.Ordinal)) return StatusSwitched;
            if (s.StartsWith("switch failed: ", StringComparison.Ordinal)) return s;
            if (s.Contains(" expired — ")) return "slot expired (Refresh or re-add it in AccountSwitcher)";
        }
        return null;
    }

    private static bool IsUnderPanel(Transform t)
    {
        for (var cur = t.parent; cur != null; cur = cur.parent)
            if (cur.gameObject.name == WindowRootName)
                return cur.parent != null && cur.parent.gameObject.name == CanvasName;
        return false;
    }

    // Walks up from the button: the nearest "Slot" ancestor, provided the chain continues to
    // accountswitcher.main directly under the Stellar window canvas. Null for any other window's button.
    private static Transform? FindSlotUnderPanel(Transform t)
    {
        Transform? slot = null;
        for (var cur = t.parent; cur != null; cur = cur.parent)
        {
            var name = cur.gameObject.name;
            if (slot == null && name == SlotName) slot = cur;
            if (name == WindowRootName)
                return cur.parent != null && cur.parent.gameObject.name == CanvasName ? slot : null;
        }
        return null;
    }

    private static string ButtonLabel(Button btn)
    {
        try { return btn.GetComponentInChildren<Text>()?.text?.Trim() ?? string.Empty; }
        catch { return string.Empty; }
    }

    // The row's label is the first Text in the Slot that is not a button caption (cell 0 = account name).
    private static string SlotLabel(Transform slot)
    {
        var texts = slot.GetComponentsInChildren<Text>();
        for (var i = 0; i < texts.Length; i++)
        {
            var t = texts[i];
            if (t == null || t.GetComponentInParent<Button>() != null) continue;
            return t.text ?? string.Empty;
        }
        return string.Empty;
    }
}
