namespace Netnol.Identity.Core;

/// <summary>
///     Provides utility methods for converting between binary and textual representations.
/// </summary>
public static class CONVERTER
{
    /// <summary>
    ///     Converts a byte array to string representation.
    /// </summary>
    /// <param name="binary">The byte buffer to be converted.</param>
    /// <returns>A string containing the representation of the data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the bytes parameter is null.</exception>
    public static string FromBinary(byte[] binary)
    {
        return binary == null
            ? throw new ArgumentNullException(nameof(binary))
            : Convert.ToHexStringLower(binary);
    }

    /// <summary>
    ///     Converts a string into a byte array.
    /// </summary>
    /// <param name="text">The string to be converted.</param>
    /// <returns>A byte array corresponding to the data in the string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the text parameter is null.</exception>
    /// <exception cref="FormatException">Thrown when the string is null.</exception>
    public static byte[] ToBinary(string text)
    {
        return text == null
            ? throw new ArgumentNullException(nameof(text))
            : Convert.FromHexString(text);
    }
}