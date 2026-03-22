namespace Netnol.Identity.Core;

/// <summary>
///     Provides utility methods for converting between binary and textual representations.
/// </summary>
public static class CONVERTER
{
    /// <summary>
    ///     Converts a byte array to its lowercase hexadecimal string representation.
    /// </summary>
    /// <param name="binary">The byte buffer to be converted.</param>
    /// <returns>A string containing the hexadecimal representation of the data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the binary parameter is null.</exception>
    public static string ToBinary(byte[] binary)
    {
        return binary == null
            ? throw new ArgumentNullException(nameof(binary))
            : Convert.ToHexString(binary).ToLowerInvariant();
    }

    /// <summary>
    ///     Converts a hexadecimal string into a byte array.
    /// </summary>
    /// <param name="text">The hexadecimal string to be converted.</param>
    /// <returns>A byte array corresponding to the data in the string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the text parameter is null.</exception>
    /// <exception cref="FormatException">Thrown when the string does not have a valid hexadecimal format.</exception>
    public static byte[] FromBinary(string text)
    {
        return text == null
            ? throw new ArgumentNullException(nameof(text))
            : Convert.FromHexString(text);
    }
}