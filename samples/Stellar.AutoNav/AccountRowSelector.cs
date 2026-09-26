using System;
using System.Collections.Generic;
using System.Globalization;

namespace Stellar.AutoNav;

/// <summary>
/// Picks which AccountSwitcher row AutoNav logs in with, from <c>STELLAR_AUTONAV_ACCOUNT</c>:
/// unset/blank → the first row; a number <c>N</c> → the N-th row (1-based, as displayed); anything else →
/// the row whose label equals it (case-insensitive), else the ONE row whose label contains it. Pure so the
/// selection rules are testable without the game.
/// </summary>
internal static class AccountRowSelector
{
    /// <summary>Env var naming the account row (1-based index or label).</summary>
    public const string EnvVar = "STELLAR_AUTONAV_ACCOUNT";

    /// <summary>Returns the 0-based row index, or -1 when nothing (or more than one row, for a substring)
    /// matches.</summary>
    public static int Resolve(string? selector, IReadOnlyList<string> labels)
    {
        if (labels.Count == 0) return -1;
        var s = selector?.Trim() ?? string.Empty;
        if (s.Length == 0) return 0;

        if (int.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out var n))
            return n >= 1 && n <= labels.Count ? n - 1 : -1;

        for (var i = 0; i < labels.Count; i++)
            if (string.Equals(labels[i]?.Trim(), s, StringComparison.OrdinalIgnoreCase)) return i;

        var hit = -1;
        for (var i = 0; i < labels.Count; i++)
        {
            if (labels[i] == null || labels[i].IndexOf(s, StringComparison.OrdinalIgnoreCase) < 0) continue;
            if (hit >= 0) return -1;   // ambiguous substring — refuse rather than guess
            hit = i;
        }
        return hit;
    }
}
