using System.Security.Cryptography;

namespace Netnol.Identity.Core;

/// <summary>
///     Provides high-performance hashing and message authentication services.
///     This class abstracts underlying cryptographic implementations to ensure
///     long-term flexibility and security.
/// </summary>
public static class HASH
{
    /// <summary>
    ///     Computes a unique 256-bit digital digest for the provided data.
    ///     Ideal for integrity verification and generating compact identity fingerprints.
    /// </summary>
    /// <param name="data">The byte array to be hashed.</param>
    /// <returns>A 32-byte hash digest.</returns>
    public static byte[] Compute256(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return SHA256.HashData(data);
    }

    /// <summary>
    ///     Computes a high-entropy 512-bit digital digest.
    ///     Recommended for Key Derivation Functions (KDF) and high-security credential hashing.
    /// </summary>
    /// <param name="data">The byte array to be hashed.</param>
    /// <returns>A 64-byte hash digest.</returns>
    public static byte[] Compute512(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return SHA512.HashData(data);
    }

    /// <summary>
    ///     Generates a Keyed-Hash Message Authentication Code (HMAC) to verify
    ///     both data integrity and origin authenticity.
    /// </summary>
    /// <param name="data">The message to be authenticated.</param>
    /// <param name="key">The secret key used for the authentication process.</param>
    /// <returns>A 32-byte authentication tag.</returns>
    public static byte[] Authenticate(byte[] data, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(key);
        return HMACSHA256.HashData(key, data);
    }

    /// <summary>
    ///     Compares two byte sequences in constant time to prevent side-channel timing attacks.
    ///     Essential for validating sensitive hashes or authentication tags.
    /// </summary>
    /// <param name="left">First byte sequence.</param>
    /// <param name="right">Second byte sequence.</param>
    /// <returns>True if the sequences match exactly; otherwise, false.</returns>
    public static bool ConstantTimeAreEqual(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        return CryptographicOperations.FixedTimeEquals(left, right);
    }
}