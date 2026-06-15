using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Stellar.Abstractions.Domain;
using Stellar.Abstractions.Plugins;
using Stellar.Abstractions.Services;
using UnityEngine;
using UnityEngine.UI;

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

/// <summary>
/// Button-discovery diagnostic and optional autonomous navigator. On every scene change and on F12, dumps all
/// active <c>UnityEngine.UI.Button</c> instances with their sibling/parent components so the game's real
/// click-handler types are visible in the BepInEx log.
///
/// When <c>STELLAR_AUTONAV=1</c>, also auto-clicks a hardcoded path sequence to drive
/// Title → Character Select → World without manual input:
///
///   Scene 1   +600 frames → btn_start_face        (Title → Login; long wait for Tencent session check via EdgeWebView)
///   LoginEvent+180 frames → rolechoose_1/btn_item (pick character)
///             +270 frames → btn_entergame         (load world)
///   Scene 7   +180 frames → btn_close_new         (dismiss welcome popup)
///
/// Clicks go through <c>Panda.ZUi.ZButton.invokeClickEvent()</c> via a HarmonyX reverse patch (see
/// <see cref="ZButtonReversePatch"/>). Demonstrates HarmonyX reverse patching and frame-delayed
/// click scheduling against hot-update UI components.
/// </summary>
public sealed partial class Plugin : IStellarPlugin
{
    public string Name => "AutoNav";

    private const int FramesToWaitAfterScene = 60;  // ~1s at 60fps; let the UI settle.

    // Path constants for the autonomous Title → Char Select → World flow.
    // Captured via the diagnostic dump — re-run the diagnostic if game UI changes.
    // The real "Start" / "Enter Game" trigger on Title. It only appears in the
    // button enumeration AFTER the Tencent session check completes (the Title
    // screen is initially in a "loading session" state with fewer active
    // buttons). btn_start_face routes to Create-Character; node_play_friends
    // opens the Friends panel; this is the one wired to actual login.
    private const string PathStartButton      = "zuiroot/UILayerMain/login_main(Clone)/anim/node_enter_game/anim_enter_game/btn_rayimg";
    private const string PathCharSlot1        = "zuiroot/UILayerFunc/face_rolechoose_window(Clone)/anim/node_right/node_rolechoose_1/btn_item";
    private const string PathEnterGame        = "zuiroot/UILayerFunc/face_rolechoose_window(Clone)/anim/node_enter/btn_entergame";
    private const string PathCloseNewbiePopup = "zuiroot/UILayerFuncPopup/newbiebackflow_popup(Clone)/anim/node_info/btn_close_new/anim/btn";

    private readonly struct PendingClick
    {
        public PendingClick(int framesRemaining, string path, string label)
        {
            FramesRemaining = framesRemaining;
            Path = path;
            Label = label;
        }
        public int FramesRemaining { get; }
        public string Path { get; }
        public string Label { get; }
    }

    private readonly List<PendingClick> _pendingClicks = new();

    private readonly IPluginServices _services;
    private IWindowControl _window = null!;
    private IHotkeyAction _toggleAction = null!;
    private readonly IDisposable[] _subscriptions;
    private readonly bool _autoNavEnabled;

    private bool _dumpPending;
    private int _framesSinceSceneEvent;
    private string _pendingSceneLabel = string.Empty;

    public Plugin(IPluginServices services)
    {
        _services = services;
        var autoNavValue = Environment.GetEnvironmentVariable("STELLAR_AUTONAV") ?? "unset";
        _autoNavEnabled = autoNavValue == "1";

        _services.Log.Info($"[AutoNav] plugin constructed (STELLAR_AUTONAV={(_autoNavEnabled ? "1 (auto-click ENABLED via Panda.ZUi.ZButton.invokeClickEvent)" : "0 (diagnostic only)")})");

        BuildWindows();
        _subscriptions = SubscribeEvents();
        _services.Framework.Update += OnUpdate;
    }

    private void BuildWindows()
    {
        _window = _services.Windows.Register(new WindowRegistration(
            new WindowSpec(
                Id:          "autonav.main",
                Title:       "AutoNav",
                DefaultRect: new WindowRect(356f, 47f, 0f, 0f),   // PillStatus hugs its content width
                Category:    WindowCategory.Tools,
                Style:       WindowPanelStyle.PillStatus)
            // PillStatus has no title bar → ShowTitleBar=false so Draggable wires whole-frame drag (drag the pill body).
            // EditModeDragOnly: it's a HUD overlay, so it moves only in Shift+` layout edit mode (drag-mode is now
            // explicit per window, no longer inferred from the overlay chrome style).
            { Draggable = true, ShowTitleBar = false, EditModeDragOnly = true },
            BuildRoot()));

        _toggleAction = _services.Hotkeys.DeclareAction(
            new HotkeyAction(
                Id:              "autonav.toggle",
                Description:     "Toggle AutoNav waypoint",
                SuggestedDefault: new KeyBinding(StellarKeyCode.F12)),
            callback: ToggleAndDump);
    }

    private IDisposable[] SubscribeEvents() => new[]
    {
        _services.GameEvents.Subscribe("Panda.Core.OnEnterSceneEvent", message =>
        {
            var sceneId = message?.ToString();
            _pendingSceneLabel = $"OnEnterScene:{sceneId ?? "(null)"}";
            _framesSinceSceneEvent = 0;
            _dumpPending = true;
            _services.Log.Info($"[AutoNav] scene event '{sceneId}'; will dump buttons in ~{FramesToWaitAfterScene} frames");

            // Auto-click triggers (STELLAR_AUTONAV=1 only).
            // Scene 1 fires when the Title screen LOADS, but the Start button
            // is not actually clickable until the Tencent session check via
            // an embedded EdgeWebView completes — typically 5–10s. Delay 600
            // frames (~10s @60fps) to clear that window. A polling-based wait
            // on Panda.ZUi.ZButton.IsDisabled would be more robust but needs
            // another reverse patch; revisit if 10s ever proves too short.
            var sceneIdStr = message?.ToString() ?? string.Empty;
            if (sceneIdStr == "1")
            {
                EnqueueClick(600, PathStartButton, "start");
            }
            else if (sceneIdStr == "7" || sceneIdStr == "8")
            {
                // Scene 7 is the in-world scene on this game version — the
                // legacy "scene 8" hand-off observed during the Phase-1 recon
                // no longer fires (game patch). The newbiebackflow popup
                // activates inside scene 7's lifecycle (confirmed by the
                // scene-7 button dump containing
                // newbiebackflow_popup(Clone)/.../btn_close_new/anim/btn).
                // Enqueue on both 7 and 8 so a future game patch that
                // restores the scene-8 hand-off keeps working.
                EnqueueClick(180, PathCloseNewbiePopup, "close-newbie");
            }
        }),
        _services.GameEvents.Subscribe("Panda.Core.LoginEvent", _ =>
        {
            EnqueueClick(180, PathCharSlot1, "char-slot-1");
            EnqueueClick(270, PathEnterGame, "enter-game");
        }),
    };

    public void Dispose()
    {
        foreach (var sub in _subscriptions)
        {
            try { sub.Dispose(); } catch { }
        }
        _services.Framework.Update -= OnUpdate;
        _toggleAction.Dispose();
        _window.Remove();
        // Phase 9a soft-cycle: undo the HarmonyX reverse patch so a re-enable
        // doesn't double-register (CreateReversePatcher throws on dup).
        ZButtonReversePatch.Unpatch();
    }

    private void ToggleAndDump()
    {
        _window.SetVisible(!_window.IsShown);
        // Keep the legacy F12 diagnostic behaviour: every toggle also dumps the
        // current active-Button list so the diagnostic capability still works
        // after migrating away from raw Input polling.
        DumpButtons("F12-manual");
    }

    private void OnUpdate(float deltaTime)
    {
        TickPendingClicks();

        if (_dumpPending)
        {
            _framesSinceSceneEvent++;
            if (_framesSinceSceneEvent >= FramesToWaitAfterScene)
            {
                _dumpPending = false;
                DumpButtons(_pendingSceneLabel);
            }
        }
    }

    // The PillStatus chip as a uGUI element tree (built once; Funcs re-pulled on the framework refresh).
    // "<accent »»> AutoNav <mode> · <queue>" — the accent chevron mirrors the game's "»» Navigating 950m";
    // mode (AUTO/OBS) + the queue count are the at-a-glance state. Per-click detail stays diagnostic (the log).
    // The PillStatus chrome hugs the content + centres it; the framework owns sizing (no manual SetSize).
    private HudElement BuildRoot()
    {
        var mode = _autoNavEnabled ? "AUTO" : "OBS";
        return new RowElement(new HudElement[]
        {
            new TextElement(() => "»»", () => _services.Theme.Colors.HudAccent, Emphasis: true),
            new TextElement(() => "AutoNav", Emphasis: true),
            new TextElement(() => mode, Emphasis: true),
            new ConditionalElement(() => _pendingClicks.Count > 0,
                new TextElement(() => $"· {_pendingClicks.Count}", () => _services.Theme.Colors.TextMuted)),
        }, Gap: 6f);
    }

    // -------------------------------------------------------------------------
    // Click queue + scheduler
    // -------------------------------------------------------------------------

    private void EnqueueClick(int delayFrames, string path, string label)
    {
        if (!_autoNavEnabled) return;
        _pendingClicks.Add(new PendingClick(delayFrames, path, label));
        _services.Log.Info($"[AutoNav] queued click '{label}' in {delayFrames} frames");
    }

    private void TickPendingClicks()
    {
        for (var i = _pendingClicks.Count - 1; i >= 0; i--)
        {
            var pc = _pendingClicks[i];
            if (pc.FramesRemaining > 1)
            {
                _pendingClicks[i] = new PendingClick(pc.FramesRemaining - 1, pc.Path, pc.Label);
            }
            else
            {
                _pendingClicks.RemoveAt(i);
                InvokeZButtonClick(pc.Path, pc.Label);
            }
        }
    }

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
            // Resolve the type + apply the reverse patch (cached/idempotent).
            if (!TryResolveZButtonInvoker())
            {
                _services.Log.Warning($"[AutoNav] CLICK FAILED ({label}): Panda.ZUi.ZButton or invokeClickEvent() not found in any loaded assembly");
                return false;
            }
            if (!ZButtonReversePatch.TryApply(_zbuttonType!, _services.Log))
            {
                _services.Log.Warning($"[AutoNav] CLICK FAILED ({label}): reverse patch could not be applied");
                return false;
            }

            return TryInvokeReversePatchOnce(path, label);
        }
        catch (Exception ex)
        {
            _services.Log.Warning($"[AutoNav] CLICK FAILED ({label}): {ex.GetType().Name}: {ex.Message}");
            return false;
        }
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
