using Xunit;

namespace Stellar.AutoNav.Tests;

// Origin: in-world scenario 2026-09-26 — fixed-delay blind clicks failed when the target was late or
// absent (char select path "not found" after the game entered the world directly). Steps now poll with
// a bounded budget.
public class RetryStepTests
{
    [Fact]
    public void Waits_the_first_delay_before_the_first_attempt()
    {
        var calls = 0;
        var step = new RetryStep("x", 2f, 1f, 5, _ => { calls++; return StepResult.Done; });
        step.Tick(1.5f);
        Assert.Equal(0, calls);
        step.Tick(0.6f);
        Assert.Equal(1, calls);
        Assert.True(step.Finished);
        Assert.Equal(StepResult.Done, step.Outcome);
        Assert.False(step.Exhausted);
    }

    [Fact]
    public void Retries_on_the_interval_until_done()
    {
        var step = new RetryStep("x", 0f, 1f, 10, attempt => attempt < 3 ? StepResult.Retry : StepResult.Done);
        step.Tick(0.01f);          // attempt 1
        step.Tick(0.5f);           // interval not elapsed
        Assert.Equal(1, step.Attempts);
        step.Tick(0.6f);           // attempt 2
        step.Tick(1f);             // attempt 3 → done
        Assert.Equal(3, step.Attempts);
        Assert.Equal(StepResult.Done, step.Outcome);
    }

    [Fact]
    public void Budget_exhaustion_is_a_failure_flagged_exhausted()
    {
        var step = new RetryStep("x", 0f, 1f, 3, _ => StepResult.Retry);
        for (var i = 0; i < 10; i++) step.Tick(1f);
        Assert.Equal(3, step.Attempts);                // bounded — never polls forever
        Assert.True(step.Finished);
        Assert.Equal(StepResult.Fail, step.Outcome);
        Assert.True(step.Exhausted);
    }

    [Fact]
    public void Explicit_failure_is_not_flagged_exhausted()
    {
        var step = new RetryStep("x", 0f, 1f, 3, _ => StepResult.Fail);
        step.Tick(0.1f);
        Assert.Equal(StepResult.Fail, step.Outcome);
        Assert.False(step.Exhausted);
    }

    [Fact]
    public void Finished_step_never_attempts_again()
    {
        var calls = 0;
        var step = new RetryStep("x", 0f, 1f, 3, _ => { calls++; return StepResult.Done; });
        step.Tick(1f); step.Tick(1f); step.Tick(1f);
        Assert.Equal(1, calls);
    }
}
