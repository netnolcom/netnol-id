using System.Security.Cryptography;

namespace Netnol.Identity.Core;

/// <summary>
///     Provides authenticated data protection services.
/// </summary>
public static class CIPHER
{
    public const int ExpectedNonceSize = 12;
    public const int ExpectedTagSize = 16;
    public const int ExpectedKeySize = 32;

    /// <summary>
    ///     Protects the specified data using a symmetric key and returns a secure capsule.
    /// </summary>
    /// <param name="data">The plaintext buffer to be protected.</param>
    /// <param name="key">The symmetric key used for protection.</param>
    /// <returns>A byte array containing the protected capsule (nonce, tag, and ciphertext).</returns>
    /// <exception cref="ArgumentNullException">Thrown when data or key is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the key size is invalid.</exception>
    public static byte[] Protect(byte[] data, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(key);

        if (key.Length != ExpectedKeySize) throw new ArgumentException($"Key must be {ExpectedKeySize} bytes.", nameof(key));

        var nonce = RandomNumberGenerator.GetBytes(ExpectedNonceSize);
        var ciphertext = new byte[data.Length];
        var tag = new byte[ExpectedTagSize];

        using var engine = new AesGcm(key, ExpectedTagSize);
        engine.Encrypt(nonce, data, ciphertext, tag);

        var capsule = new byte[ExpectedNonceSize + ExpectedTagSize + ciphertext.Length];

        Buffer.BlockCopy(nonce, 0, capsule, 0, ExpectedNonceSize);
        Buffer.BlockCopy(tag, 0, capsule, ExpectedNonceSize, ExpectedTagSize);
        Buffer.BlockCopy(ciphertext, 0, capsule, ExpectedNonceSize + ExpectedTagSize, ciphertext.Length);

        return capsule;
    }

    /// <summary>
    ///     Unprotects a secure capsule and verifies its integrity.
    /// </summary>
    /// <param name="capsule">The protected buffer to be restored.</param>
    /// <param name="key">The symmetric key used for restoration.</param>
    /// <returns>The original decrypted and verified plaintext buffer.</returns>
    /// <exception cref="ArgumentNullException">Thrown when capsule or key is null.</exception>
    /// <exception cref="CryptographicException">Thrown when restoration or integrity verification fails.</exception>
    public static byte[] Unprotect(byte[] capsule, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(capsule);
        ArgumentNullException.ThrowIfNull(key);

        if (capsule.Length < ExpectedNonceSize + ExpectedTagSize) throw new ArgumentException("Invalid capsule format.");

        var ciphertextLength = capsule.Length - ExpectedNonceSize - ExpectedTagSize;

        var nonce = new byte[ExpectedNonceSize];
        var tag = new byte[ExpectedTagSize];
        var ciphertext = new byte[ciphertextLength];

        Buffer.BlockCopy(capsule, 0, nonce, 0, ExpectedNonceSize);
        Buffer.BlockCopy(capsule, ExpectedNonceSize, tag, 0, ExpectedTagSize);
        Buffer.BlockCopy(capsule, ExpectedNonceSize + ExpectedTagSize, ciphertext, 0, ciphertextLength);

        var plaintext = new byte[ciphertextLength];

        using var engine = new AesGcm(key, ExpectedTagSize);
        engine.Decrypt(nonce, ciphertext, tag, plaintext);

        return plaintext;
    }
}