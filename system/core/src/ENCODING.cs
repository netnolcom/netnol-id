using System.Text;

namespace Netnol.Identity.Core;

/// <summary>
///     Provides text and binary data conversion services for the identity layer.
///     Enables transformation between human-readable text and its raw byte representation.
/// </summary>
public static class ENCODING
{
    /// <summary>Converts a text string into a sequence of bytes.</summary>
    /// <param name="text">The text to convert. May be null or empty.</param>
    /// <returns>A byte array representing the input text. If the input is null, an empty array is returned.</returns>
    public static byte[]? ToBinary(string text)
    {
        return text is { Length: > 0 }
            ? Encoding.UTF8.GetBytes(text)
            : null;
    }

    /// <summary>Restores a text string from its binary representation.</summary>
    /// <param name="data">The byte sequence to convert back to text.</param>
    /// <returns>The reconstructed text string. If the input is null, an empty string is returned.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided byte sequence does not represent valid text. </exception>
    public static string? FromBinary(byte[] data)
    {
        return data is { Length: > 0 }
            ? Encoding.UTF8.GetString(data)
            : null;
    }
}