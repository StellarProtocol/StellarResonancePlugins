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
/// Button-discovery diagnostic and optional autonomous navigator. On every scene change and on F12, dumps all
/// active <c>UnityEngine.UI.Button</c> instances with their sibling/parent components so the game's real
/// click-handler types are visible in the BepInEx log.
///
/// When <c>STELLAR_AUTONAV=1</c>, also auto-clicks a hardcoded path sequence to drive
/// Title → Character Select → World without manual input:
///
///   Scene 1   +10s   → AccountSwitcher row "Login" button when that plugin is loaded (see Plugin.Login.cs,
///                      row chosen by STELLAR_AUTONAV_ACCOUNT), else the native start button
///   LoginEvent       → poll: node_rolechoose_N/btn_item, then btn_entergame (see Plugin.CharSelect.cs;
///                      stands down if the game enters the world without character select)
///   Scene 7   +3s    → btn_close_new         (dismiss welcome popup)
///
/// Clicks go through <c>Panda.ZUi.ZButton.invokeClickEvent()</c> via a HarmonyX reverse patch (see
/// <see cref="ZButtonReversePatch"/>). Since the framework's own tick is gated off until
/// logged-in + in-world, AutoNav drives its scheduling from a self-installed HarmonyX postfix
/// on <c>Panda.Core.Game.Update(float)</c> (see <see cref="GameUpdatePulse"/>) instead of
/// <c>Framework.Update</c> — falling back to the framework tick if that patch fails. Scheduling
/// is time-based (seconds, using the game's own deltaTime) rather than frame-count-based, so the
/// waits stay correct regardless of the client's frame rate (the test prefix runs uncapped).
/// Demonstrates HarmonyX reverse patching, a self-installed per-frame postfix, and time-delayed
/// click scheduling against hot-update UI components.
/// </summary>
public sealed partial class Plugin : IStellarPlugin
{
    public string Name => "AutoNav";

    private const float SecondsToWaitAfterScene = 1f;  // let the UI settle.

    // Path constants for the autonomous Title → Char Select → World flow.
    // Captured via the diagnostic dump — re-run the diagnostic if game UI changes.
    // The real "Start" / "Enter Game" trigger on Title. It only appears in the
    // button enumeration AFTER the Tencent session check completes (the Title
    // screen is initially in a "loading session" state with fewer active
    // buttons). btn_start_face routes to Create-Character; node_play_friends
    // opens the Friends panel; this is the one wired to actual login.
    private const string PathStartButton      = "zuiroot/UILayerMain/login_main(Clone)/anim/node_enter_game/anim_enter_game/btn_rayimg";
    private const string PathCloseNewbiePopup = "zuiroot/UILayerFuncPopup/newbiebackflow_popup(Clone)/anim/node_info/btn_close_new/anim/btn";

    private readonly struct PendingClick
    {
        public PendingClick(float secondsRemaining, string path, string label)
        {
            SecondsRemaining = secondsRemaining;
            Path = path;
            Label = label;
        }
        public float SecondsRemaining { get; }
        public string Path { get; }
        public string Label { get; }
    }

    private readonly List<PendingClick> _pendingClicks = new();

    private readonly IPluginServices _services;
    private IWindowControl _window = null!;
    private IHotkeyAction _toggleAction = null!;
    private readonly IDisposable[] _subscriptions;
    private readonly bool _autoNavEnabled;
    private bool _usesPulse;

    private bool _dumpPending;
    private float _secondsSinceSceneEvent;
    private string _pendingSceneLabel = string.Empty;

    public Plugin(IPluginServices services)
    {
        _services = services;
        var autoNavValue = Environment.GetEnvironmentVariable("STELLAR_AUTONAV") ?? "unset";
        _autoNavEnabled = autoNavValue == "1";

        _services.Log.Info($"[AutoNav] plugin constructed (STELLAR_AUTONAV={(_autoNavEnabled ? "1 (auto-click ENABLED via Panda.ZUi.ZButton.invokeClickEvent)" : "0 (diagnostic only)")})");

        BuildWindows();
        _subscriptions = SubscribeEvents();

        // The framework tick is gated off until logged-in + in-world (scene-transition
        // gate), so it cannot drive Title/Char-Select clicks. In AUTO mode, pulse from a
        // Game.Update postfix instead; fall back to the framework tick when the patch
        // fails or in diagnostic-only mode (where in-world dumps still work).
        if (_autoNavEnabled && GameUpdatePulse.TryApply(_services.Log))
        {
            GameUpdatePulse.OnPulse += OnUpdate;
            _usesPulse = true;
        }
        else
        {
            _services.Framework.Update += OnUpdate;
        }
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
            // ShouldRender (required since framework 2.x): always — the pill is the at-a-glance AUTO/OBS state on
            // every screen, Title and Char Select included.
            { Draggable = true, ShowTitleBar = false, EditModeDragOnly = true, ShouldRender = () => true },
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
            _secondsSinceSceneEvent = 0;
            _dumpPending = true;
            _services.Log.Info($"[AutoNav] scene event '{sceneId}'; will dump buttons in ~{SecondsToWaitAfterScene:0.#}s");

            // Auto-click triggers (STELLAR_AUTONAV=1 only).
            // Scene 1 fires when the Title screen LOADS, but the Start button
            // is not actually clickable until the Tencent session check via
            // an embedded EdgeWebView completes — typically 5–10s. Delay 10s
            // to clear that window. A polling-based wait on
            // Panda.ZUi.ZButton.IsDisabled would be more robust but needs
            // another reverse patch; revisit if 10s ever proves too short.
            var sceneIdStr = message?.ToString() ?? string.Empty;
            if (sceneIdStr == "1")
            {
                BeginLogin();
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
                OnWorldEntered();
                EnqueueClick(3f, PathCloseNewbiePopup, "close-newbie");
            }
        }),
        _services.GameEvents.Subscribe("Panda.Core.LoginEvent", _ => BeginCharacterSelect()),
    };

    public void Dispose()
    {
        foreach (var sub in _subscriptions)
        {
            try { sub.Dispose(); } catch { }
        }
        if (_usesPulse) { GameUpdatePulse.OnPulse -= OnUpdate; GameUpdatePulse.Unpatch(); }
        else { _services.Framework.Update -= OnUpdate; }
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
        TickPendingClicks(deltaTime);
        TickSteps(deltaTime);

        if (_dumpPending)
        {
            _secondsSinceSceneEvent += deltaTime;
            if (_secondsSinceSceneEvent >= SecondsToWaitAfterScene)
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
            new ConditionalElement(() => _pendingClicks.Count + _steps.Count > 0,
                new TextElement(() => $"· {_pendingClicks.Count + _steps.Count}", () => _services.Theme.Colors.TextMuted)),
        }, Gap: 6f);
    }

    // -------------------------------------------------------------------------
    // Click queue + scheduler
    // -------------------------------------------------------------------------

    private void EnqueueClick(float delaySeconds, string path, string label)
    {
        if (!_autoNavEnabled) return;
        _pendingClicks.Add(new PendingClick(delaySeconds, path, label));
        _services.Log.Info($"[AutoNav] queued click '{label}' in {delaySeconds:0.#}s");
    }

    private void TickPendingClicks(float dt)
    {
        for (var i = _pendingClicks.Count - 1; i >= 0; i--)
        {
            var pc = _pendingClicks[i];
            if (pc.SecondsRemaining > dt)
            {
                _pendingClicks[i] = new PendingClick(pc.SecondsRemaining - dt, pc.Path, pc.Label);
            }
            else
            {
                _pendingClicks.RemoveAt(i);
                InvokeZButtonClick(pc.Path, pc.Label);
            }
        }
    }
}
