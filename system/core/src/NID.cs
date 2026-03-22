using System.Text.RegularExpressions;

namespace Netnol.Identity.Core;

/// <summary>
///     Static utility for Network Identity names (Usernames).
/// </summary>
public static class NID
{
    /// <summary>
    ///     Validates NID rules: 3-30 chars, alphanumeric start/end, no consecutive symbols.
    /// </summary>
    /// <param name="input">The raw username string to validate.</param>
    /// <returns>True if the normalized input follows all system rules.</returns>
    public static bool IsValid(string input)
    {
        var nid = Normalize(input);

        if (nid.Length is < 3 or > 30)
            return false;

        if (!Regex.IsMatch(nid, @"^[a-z0-9.-]+$"))
            return false;

        if (nid.Contains("--") || nid.Contains("..") || nid.Contains("-.") || nid.Contains(".-"))
            return false;

        if ("-.".Contains(nid[0]) || "-.".Contains(nid[^1]))
            return false;

        return true;
    }

    /// <summary>
    ///     Normalizes input by trimming whitespace and converting to lowercase.
    ///     Returns an empty string if input is null or whitespace.
    /// </summary>
    /// <param name="input">The raw string to be normalized.</param>
    /// <returns>A sanitized, lowercase string ready for validation or storage.</returns>
    public static string Normalize(string input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? string.Empty
            : input.Trim().ToLowerInvariant();
    }
}