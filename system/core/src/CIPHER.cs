using System.Security.Cryptography;

namespace Netnol.Identity.Core;

/// <summary>
///     Provides authenticated data protection services.
/// </summary>
public static class CIPHER
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int KeySize = 32;

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

        if (key.Length != KeySize) throw new ArgumentException($"Key must be {KeySize} bytes.", nameof(key));

        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var ciphertext = new byte[data.Length];
        var tag = new byte[TagSize];

        using var engine = new AesGcm(key, TagSize);
        engine.Encrypt(nonce, data, ciphertext, tag);

        var capsule = new byte[NonceSize + TagSize + ciphertext.Length];

        Buffer.BlockCopy(nonce, 0, capsule, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, capsule, NonceSize, TagSize);
        Buffer.BlockCopy(ciphertext, 0, capsule, NonceSize + TagSize, ciphertext.Length);

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

        if (capsule.Length < NonceSize + TagSize) throw new ArgumentException("Invalid capsule format.");

        var ciphertextLength = capsule.Length - NonceSize - TagSize;

        var nonce = new byte[NonceSize];
        var tag = new byte[TagSize];
        var ciphertext = new byte[ciphertextLength];

        Buffer.BlockCopy(capsule, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(capsule, NonceSize, tag, 0, TagSize);
        Buffer.BlockCopy(capsule, NonceSize + TagSize, ciphertext, 0, ciphertextLength);

        var plaintext = new byte[ciphertextLength];

        using var engine = new AesGcm(key, TagSize);
        engine.Decrypt(nonce, ciphertext, tag, plaintext);

        return plaintext;
    }
}