using System;
using System.Collections.Generic;
using Stellar.Abstractions.Domain;
using Stellar.Abstractions.Plugins;
using Stellar.Abstractions.Services;

namespace Stellar.DebugInfo;

/// <summary>
/// Minimal framework health overlay. Shows the current frame counter, a rolling FPS average,
/// login state, current scene name, and a timestamped log of recent <see cref="IClientState"/>
/// and game-bus events. Demonstrates the uGUI HUD toolkit, event subscription, and the per-tick
/// Update pattern; intended as a low-noise sanity-check that the framework is running.
///
/// Hotkey: F10 toggles visibility (user can rebind).
/// </summary>
public sealed class Plugin : IStellarPlugin
{
    private const int MaxEventLines = 8;
    private const float FpsRefreshInterval = 0.5f;
    private const int FpsSampleWindow = 60;

    public string Name => "DebugInfo";

    private readonly IPluginServices _services;
    private readonly IHudHandle _hud;
    private readonly IHotkeyAction _toggleAction;
    private readonly IDisposable[] _subscriptions;

    // Phase 9a soft-cycle: captured handlers so Dispose can -=.
    private Action? _onLogin;
    private Action? _onLogout;
    private Action<string?>? _onSceneChanged;

    // Event log — thread-safe queue drained on the game thread into a stable snapshot array
    // the HUD's element Funcs read (refreshed at the toolkit's ~10 Hz poll).
    private readonly object _gate = new();
    private readonly Queue<string> _pending = new();
    private readonly LinkedList<string> _live = new();
    private string[] _snapshot = Array.Empty<string>();

    private readonly Queue<float> _frameSamples = new();
    private float _fpsAccumulator;
    private string _fpsLabel = "--";

    public Plugin(IPluginServices services)
    {
        _services = services;
        _services.Log.Info("[DebugInfo] plugin constructed");

        // Variable-length events list: MaxEventLines fixed slots, the first N shown by the
        // toolkit's ListElement (no IMGUI fixed-slot padding needed).
        var eventSlots = new HudElement[MaxEventLines];
        for (var i = 0; i < MaxEventLines; i++)
        {
            var idx = i;   // capture per-slot index
            eventSlots[i] = new TextElement(() => idx < _snapshot.Length ? "  " + _snapshot[idx] : "");
        }

        _hud = _services.Hud.Register(new HudSpec(
            Id: "debuginfo.main",
            Anchor: HudAnchor.FreeOverlay,
            DefaultRect: new WindowRect(1976f, 25f, 237f, 231f),
            Root: new ColumnElement(new HudElement[]
            {
                new TextElement(() => $"Framework frame: {_services.Framework.FrameCount}"),
                new TextElement(() => _fpsLabel),
                new TextElement(() => $"IsLoggedIn: {_services.ClientState.IsLoggedIn}"),
                new TextElement(() => $"Scene: {_services.ClientState.CurrentSceneName ?? "(unknown)"}"),
                new TextElement(() => "Recent events:", Emphasis: true),
                new ListElement(() => _snapshot.Length, eventSlots),
            }, Gap: 2f)));

        _toggleAction = _services.Hotkeys.DeclareAction(
            new HotkeyAction(
                Id:              "debuginfo.toggle",
                Description:     "Toggle DebugInfo",
                SuggestedDefault: new KeyBinding(StellarKeyCode.F10)),
            callback: () => _hud.SetVisible(!_hud.IsShown));

        _services.Framework.Update += OnUpdate;
        _onLogin        = () => RecordEvent("Login");
        _onLogout       = () => RecordEvent("Logout");
        _onSceneChanged = sceneName => RecordEvent($"Scene -> {sceneName ?? "(null)"}");
        _services.ClientState.Login        += _onLogin;
        _services.ClientState.Logout       += _onLogout;
        _services.ClientState.SceneChanged += _onSceneChanged;

        _subscriptions = new[]
        {
            _services.GameEvents.Subscribe("Panda.Core.AfterGameInitEvent", _ => RecordEvent("AfterGameInitEvent")),
            _services.GameEvents.Subscribe("Panda.Core.LoginEvent",         _ => RecordEvent("LoginEvent (event)")),
            _services.GameEvents.Subscribe("Panda.Core.OnEnterSceneEvent",  message => RecordEvent($"OnEnterSceneEvent: {message}")),
        };
    }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
        {
            try { subscription.Dispose(); }
            catch { /* disposal must not throw */ }
        }
        _services.Framework.Update -= OnUpdate;
        if (_onLogin        is not null) _services.ClientState.Login        -= _onLogin;
        if (_onLogout       is not null) _services.ClientState.Logout       -= _onLogout;
        if (_onSceneChanged is not null) _services.ClientState.SceneChanged -= _onSceneChanged;
        _toggleAction.Dispose();
        _hud.Remove();
    }

    private void RecordEvent(string entry)
    {
        var stamped = $"{DateTime.Now:HH:mm:ss} {entry}";
        lock (_gate)
        {
            _pending.Enqueue(stamped);
        }
        _services.Log.Info($"[DebugInfo] {stamped}");
    }

    private void OnUpdate(float deltaTime)
    {
        DrainPendingEvents();
        UpdateFpsLabel(deltaTime);
        _hud.MarkDirty();
    }

    private void DrainPendingEvents()
    {
        var changed = false;
        lock (_gate)
        {
            while (_pending.Count > 0)
            {
                _live.AddFirst(_pending.Dequeue());
                while (_live.Count > MaxEventLines)
                {
                    _live.RemoveLast();
                }
                changed = true;
            }
        }
        if (changed)
        {
            _snapshot = ToArray(_live);
        }
    }

    private void UpdateFpsLabel(float deltaTime)
    {
        _fpsAccumulator += deltaTime;
        _frameSamples.Enqueue(deltaTime);
        while (_frameSamples.Count > FpsSampleWindow)
        {
            _frameSamples.Dequeue();
        }
        if (_fpsAccumulator < FpsRefreshInterval)
        {
            return;
        }
        _fpsAccumulator = 0f;

        var sum = 0f;
        foreach (var sample in _frameSamples)
        {
            sum += sample;
        }
        var average = _frameSamples.Count > 0 ? sum / _frameSamples.Count : 0f;
        _fpsLabel = average > 0 ? $"{1f / average:0.0} fps  ({average * 1000f:0.0} ms)" : "--";
    }

    private static string[] ToArray(LinkedList<string> source)
    {
        var array = new string[source.Count];
        var index = 0;
        foreach (var item in source)
        {
            array[index++] = item;
        }
        return array;
    }
}
