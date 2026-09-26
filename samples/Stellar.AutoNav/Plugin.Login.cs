using System;
using System.Collections.Generic;

namespace Stellar.AutoNav;

/// <summary>
/// Title-screen login. With the AccountSwitcher plugin loaded, its "suppress native HaoPlay login" setting
/// overrides the game's SDK login, so a bare native <c>start</c> click never logs in. AutoNav first presses the
/// Login button of a saved-account row on AccountSwitcher's panel (row picked by
/// <see cref="AccountRowSelector.EnvVar"/>) — that only hands the saved session to the login VM (the SDK-login
/// half) — and, once the panel reports <c>switched</c>, presses the native start button to enter the game,
/// exactly as a player does. Without AccountSwitcher the native start click is used unchanged.
/// </summary>
public sealed partial class Plugin
{
    private const string AccountLoginLabel = "account-login";
    private const float LoginFirstDelaySeconds = 10f;    // same session-check settle as the native start click
    private const float AccountLoginIntervalSeconds = 2f;
    private const int AccountLoginMaxAttempts = 30;      // ~60 s of polling for the panel to mount
    private const string AccountStatusLabel = "account-login-status";
    private const float StartAfterSwitchSeconds = 3f;     // let the login VM take the injected session

    private readonly List<RetryStep> _steps = new();

    // Scene 1 (Title) entry point.
    private void BeginLogin()
    {
        if (!_autoNavEnabled) return;
        if (!AccountSwitcherPanel.IsLoaded())
        {
            _services.Log.Info("[AutoNav] login path: native start button (AccountSwitcher not loaded)");
            EnqueueClick(LoginFirstDelaySeconds, PathStartButton, "start");
            return;
        }
        var selector = Environment.GetEnvironmentVariable(AccountRowSelector.EnvVar);
        _services.Log.Info($"[AutoNav] login path: AccountSwitcher panel ({AccountRowSelector.EnvVar}="
            + $"{(string.IsNullOrWhiteSpace(selector) ? "unset → row 1" : $"'{selector!.Trim()}'")})");
        AddStep(new RetryStep(AccountLoginLabel, LoginFirstDelaySeconds, AccountLoginIntervalSeconds,
            AccountLoginMaxAttempts, attempt => TryAccountLogin(attempt, selector)));
    }

    private StepResult TryAccountLogin(int attempt, string? selector)
    {
        var rows = AccountSwitcherPanel.FindLoginRows();
        if (rows.Count == 0)
        {
            if (attempt == 1) _services.Log.Info("[AutoNav] waiting for the AccountSwitcher panel to show a Login row…");
            return StepResult.Retry;
        }
        var labels = new string[rows.Count];
        for (var i = 0; i < rows.Count; i++) labels[i] = rows[i].Label;
        var index = AccountRowSelector.Resolve(selector, labels);
        if (index < 0)
        {
            WarnFail(AccountLoginLabel, $"{AccountRowSelector.EnvVar}='{selector}' matched no single row of {rows.Count}");
            return StepResult.Fail;
        }
        rows[index].Login.onClick.Invoke();
        _services.Log.Info($"[AutoNav] CLICK '{AccountLoginLabel}' via AccountSwitcher panel (row {index + 1} of {rows.Count})");
        AddStep(new RetryStep(AccountStatusLabel, 0.5f, 0.5f, 6, _ => ReportAccountSwitchStatus()));
        return StepResult.Done;
    }

    // AccountSwitcher reports the switch only on its panel's status line; echo its KIND (never the account
    // label) so the log proves whether the injected session was accepted, then enter the game.
    private StepResult ReportAccountSwitchStatus()
    {
        var kind = AccountSwitcherPanel.ReadStatusKind();
        if (kind == null) return StepResult.Retry;
        _services.Log.Info($"[AutoNav] AccountSwitcher status after login click: {kind}");
        if (kind != AccountSwitcherPanel.StatusSwitched)
        {
            WarnFail(AccountLoginLabel, $"AccountSwitcher did not switch ({kind})");
            return StepResult.Fail;
        }
        EnqueueClick(StartAfterSwitchSeconds, PathStartButton, "start");
        return StepResult.Done;
    }

    private void AddStep(RetryStep step) => _steps.Add(step);

    private void CancelSteps(string label)
    {
        for (var i = _steps.Count - 1; i >= 0; i--)
            if (_steps[i].Label == label) _steps.RemoveAt(i);
    }

    private void TickSteps(float dt)
    {
        for (var i = _steps.Count - 1; i >= 0; i--)
        {
            // Index may shift if an attempt added a step; re-read defensively.
            if (i >= _steps.Count) continue;
            var step = _steps[i];
            step.Tick(dt);
            if (!step.Finished) continue;
            _steps.Remove(step);
            if (step.Exhausted) OnStepExhausted(step);
        }
    }

    private void OnStepExhausted(RetryStep step)
    {
        if (step.Label == AccountStatusLabel)
        {
            // No status line was readable (panel re-laid out / closed) — press start anyway; a real failure
            // then surfaces as no LoginEvent (scenario timeout) rather than a silent stall here.
            _services.Log.Info("[AutoNav] AccountSwitcher status after login click: (no status line seen) — pressing start anyway");
            EnqueueClick(StartAfterSwitchSeconds, PathStartButton, "start");
            return;
        }
        WarnFail(step.Label, $"gave up after {step.Attempts} attempts — target never appeared");
        DumpButtons($"after {step.Label} gave up");
    }
}
