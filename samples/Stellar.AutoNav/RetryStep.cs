using System;

namespace Stellar.AutoNav;

/// <summary>Outcome of one <see cref="RetryStep"/> attempt.</summary>
internal enum StepResult
{
    /// <summary>The step did its work (or legitimately had nothing to do) — drop it.</summary>
    Done,
    /// <summary>The target is not there yet — try again after the step's interval.</summary>
    Retry,
    /// <summary>The step cannot succeed (e.g. a selector matched nothing) — drop it and report.</summary>
    Fail,
}

/// <summary>
/// A time-based, bounded polling step: waits <c>firstDelay</c> seconds, then invokes its attempt every
/// <c>interval</c> seconds until it returns <see cref="StepResult.Done"/>/<see cref="StepResult.Fail"/> or
/// <c>maxAttempts</c> is exhausted (reported as a failure). Pure (no Unity) so the scheduling contract is
/// testable; the game-facing work lives in the attempt delegate.
/// </summary>
internal sealed class RetryStep
{
    private readonly Func<int, StepResult> _attempt;
    private readonly float _interval;
    private readonly int _maxAttempts;
    private float _secondsUntilNext;

    public RetryStep(string label, float firstDelay, float interval, int maxAttempts, Func<int, StepResult> attempt)
    {
        Label = label;
        _secondsUntilNext = firstDelay;
        _interval = interval;
        _maxAttempts = Math.Max(1, maxAttempts);
        _attempt = attempt;
    }

    /// <summary>Log label (e.g. <c>account-login</c>).</summary>
    public string Label { get; }

    /// <summary>Attempts made so far.</summary>
    public int Attempts { get; private set; }

    /// <summary>True once the step finished (any outcome).</summary>
    public bool Finished { get; private set; }

    /// <summary>Final outcome; <see cref="StepResult.Retry"/> while still running, <see cref="StepResult.Fail"/>
    /// when the attempt budget ran out.</summary>
    public StepResult Outcome { get; private set; } = StepResult.Retry;

    /// <summary>True when the step failed because every attempt returned <see cref="StepResult.Retry"/>
    /// (the attempt itself never reported a failure reason).</summary>
    public bool Exhausted { get; private set; }

    /// <summary>Advance by <paramref name="dt"/> seconds; runs at most one attempt per call.</summary>
    public void Tick(float dt)
    {
        if (Finished) return;
        _secondsUntilNext -= dt;
        if (_secondsUntilNext > 0f) return;

        Attempts++;
        var result = _attempt(Attempts);
        if (result == StepResult.Retry && Attempts < _maxAttempts)
        {
            _secondsUntilNext = _interval;
            return;
        }
        Finished = true;
        Exhausted = result == StepResult.Retry;
        Outcome = Exhausted ? StepResult.Fail : result;
    }
}
