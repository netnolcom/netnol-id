using System.Buffers.Binary;
using System.Globalization;
using System.Security.Cryptography;

namespace Netnol.Identity.Core;

/// <summary>
///     Static utility for 128-bit Object Identifiers (64b Time Ticks + 64b Crypto Random).
///     Returns and manipulates native UInt128.
/// </summary>
public static class OID
{
    /// <summary>
    ///     Generates a new 128-bit OID.
    ///     High cost: Involves Cryptographic Random and System Clock.
    /// </summary>
    public static UInt128 New()
    {
        Span<byte> buffer = RandomNumberGenerator.GetBytes(16);
        BinaryPrimitives.WriteInt64BigEndian(buffer[..8], DateTime.UtcNow.Ticks);
        return BinaryPrimitives.ReadUInt128BigEndian(buffer);
    }

    /// <summary>
    ///     Converts a 32-character Hex string back to a native UInt128.
    ///     Low cost: Pure numeric parsing.
    /// </summary>
    public static UInt128 Parse(string hex)
    {
        return UInt128.Parse(hex, NumberStyles.HexNumber);
    }

    /// <summary>
    ///     Converts the native UInt128 to a 32-character Hex string.
    ///     Low cost: Hex formatting.
    /// </summary>
    public static string ToString(UInt128 value)
    {
        return value.ToString("x32");
    }
}