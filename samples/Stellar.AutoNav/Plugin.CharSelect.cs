using UnityEngine;

namespace Stellar.AutoNav;

/// <summary>
/// Character select → world. Polls for the role-choose window instead of firing blind fixed-delay clicks:
/// the window can open late, and on the current client it may not open at all (the game can enter the world
/// straight after login), in which case the steps stand down when the in-world scene arrives instead of
/// reporting a missing path.
/// </summary>
public sealed partial class Plugin
{
    private const string CharSlotLabel = "char-slot-1";
    private const string EnterGameLabel = "enter-game";
    private const string PathRoleChooseWindow = "zuiroot/UILayerFunc/face_rolechoose_window(Clone)";
    private const string RoleSlotPrefix = "node_rolechoose_";
    private const string RoleSlotButton = "btn_item";
    private const string EnterGameButton = "btn_entergame";

    private bool _worldEntered;

    // Panda.Core.LoginEvent entry point.
    private void BeginCharacterSelect()
    {
        if (!_autoNavEnabled) return;
        _worldEntered = false;
        CancelSteps(CharSlotLabel);
        CancelSteps(EnterGameLabel);
        AddStep(new RetryStep(CharSlotLabel, 1f, 1f, 30, _ => TryCharSlot()));
    }

    // Scene 7/8 (in-world) — any still-pending character-select step is moot.
    private void OnWorldEntered()
    {
        _worldEntered = true;
        if (_steps.Exists(s => s.Label == CharSlotLabel))
            _services.Log.Info("[AutoNav] char select skipped: the game entered the world without showing character select");
        CancelSteps(CharSlotLabel);
        CancelSteps(EnterGameLabel);
    }

    private StepResult TryCharSlot()
    {
        if (_worldEntered) return StepResult.Done;
        var window = FindByPath(PathRoleChooseWindow);
        if (window == null || !window.gameObject.activeInHierarchy) return StepResult.Retry;
        var slot = FindFirstSlotButton(window);
        if (slot == null) return StepResult.Retry;   // window still animating in
        if (!ClickZButtonOn(slot, CharSlotLabel)) return StepResult.Fail;
        AddStep(new RetryStep(EnterGameLabel, 1.5f, 1f, 10, _ => TryEnterGame()));
        return StepResult.Done;
    }

    private StepResult TryEnterGame()
    {
        if (_worldEntered) return StepResult.Done;
        var window = FindByPath(PathRoleChooseWindow);
        if (window == null || !window.gameObject.activeInHierarchy)
        {
            _services.Log.Info($"[AutoNav] {EnterGameLabel} not needed: character select closed after the slot click (world loading)");
            return StepResult.Done;
        }
        var button = FindDescendant(window, EnterGameButton);
        if (button == null || !button.gameObject.activeInHierarchy) return StepResult.Retry;
        return ClickZButtonOn(button, EnterGameLabel) ? StepResult.Done : StepResult.Fail;
    }

    // First active node_rolechoose_<n>/btn_item, lowest n first (the old fixed path was node_rolechoose_1/btn_item).
    private static Transform? FindFirstSlotButton(Transform window)
    {
        var nodes = new System.Collections.Generic.List<Transform>();
        CollectByPrefix(window, RoleSlotPrefix, nodes);
        nodes.Sort((a, b) => string.CompareOrdinal(a.gameObject.name, b.gameObject.name));   // _1 first
        foreach (var node in nodes)
        {
            if (!node.gameObject.activeInHierarchy) continue;
            var btn = FindDescendant(node, RoleSlotButton);
            if (btn != null && btn.gameObject.activeInHierarchy) return btn;
        }
        return null;
    }

    private static Transform? FindByPath(string path)
    {
        var slash = path.IndexOf('/');
        var root = GameObject.Find(slash < 0 ? path : path.Substring(0, slash));
        if (root == null) return null;
        return slash < 0 ? root.transform : root.transform.Find(path.Substring(slash + 1));
    }

    private static Transform? FindDescendant(Transform root, string name)
    {
        for (var i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            if (child.gameObject.name == name) return child;
            var deeper = FindDescendant(child, name);
            if (deeper != null) return deeper;
        }
        return null;
    }

    private static void CollectByPrefix(Transform root, string prefix, System.Collections.Generic.List<Transform> into)
    {
        for (var i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            if (child.gameObject.name.StartsWith(prefix, System.StringComparison.Ordinal)) into.Add(child);
            else CollectByPrefix(child, prefix, into);
        }
    }
}
