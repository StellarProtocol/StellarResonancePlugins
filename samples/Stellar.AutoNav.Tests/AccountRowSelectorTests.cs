using Xunit;

namespace Stellar.AutoNav.Tests;

// Origin: owner requirement 2026-09-26 — "the test driven script suppose to support login by account
// manager". STELLAR_AUTONAV_ACCOUNT picks the AccountSwitcher row AutoNav logs in with.
public class AccountRowSelectorTests
{
    private static readonly string[] Rows = { "Main alt", "Test account", "tester2@example.com" };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Unset_selects_the_first_row(string? selector) =>
        Assert.Equal(0, AccountRowSelector.Resolve(selector, Rows));

    [Theory]
    [InlineData("1", 0)]
    [InlineData("2", 1)]
    [InlineData(" 3 ", 2)]
    public void Number_is_a_one_based_display_index(string selector, int expected) =>
        Assert.Equal(expected, AccountRowSelector.Resolve(selector, Rows));

    [Theory]
    [InlineData("0")]
    [InlineData("4")]
    [InlineData("-1")]
    public void Out_of_range_number_matches_nothing(string selector) =>
        Assert.Equal(-1, AccountRowSelector.Resolve(selector, Rows));

    [Fact]
    public void Exact_label_wins_case_insensitively() =>
        Assert.Equal(1, AccountRowSelector.Resolve("TEST ACCOUNT", Rows));

    [Fact]
    public void Unique_substring_matches() =>
        Assert.Equal(2, AccountRowSelector.Resolve("tester2", Rows));

    [Fact]
    public void Ambiguous_substring_is_refused_not_guessed() =>
        Assert.Equal(-1, AccountRowSelector.Resolve("t", new[] { "alt one", "alt two" }));

    [Fact]
    public void Exact_match_beats_an_ambiguous_substring() =>
        Assert.Equal(0, AccountRowSelector.Resolve("alt", new[] { "alt", "alt two" }));

    [Fact]
    public void No_rows_matches_nothing() =>
        Assert.Equal(-1, AccountRowSelector.Resolve(null, System.Array.Empty<string>()));
}
