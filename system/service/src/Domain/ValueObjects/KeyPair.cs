using Netnol.Identity.Core;

namespace Netnol.Identity.Service.Domain.ValueObjects;

/// <summary>
///     Represents the identity's cryptographic key pair, including identification hashes
///     and protected private keys for different security contexts.
/// </summary>
public readonly record struct KeyPair
{
    /// <summary>Expected size of a public key in bytes.</summary>
    public const uint PublicKeySize = KEM.ExpectedPublicKeyLength;

    /// <summary>Expected size of a private key in bytes.</summary>
    public const uint PrivateKeySize = KEM.ExpectedPrivateKeyLength;

    /// <summary>
    ///     Expected size of an encrypted private key: private key size + cipher tag + cipher nonce.
    /// </summary>
    public const uint EncryptedPrivateKeySize = PrivateKeySize + CIPHER.ExpectedTagSize + CIPHER.ExpectedNonceSize;

    /// <summary>Expected size of a hash (SHA‑512) in bytes.</summary>
    public const uint HashSize = HASH.ExpectedCompute512Size;

    /// <summary>
    ///     Initializes a new <see cref="KeyPair" /> with strict size validation for keys and hashes.
    /// </summary>
    /// <param name="publicKey">The public key material.</param>
    /// <param name="publicHash">The hash of the public key.</param>
    /// <param name="privateWithPassword">The private key encrypted with password-derived key.</param>
    /// <param name="privateWithSeed">The private key encrypted with seed-derived key.</param>
    /// <param name="privateHash">The hash of the private key material.</param>
    /// <exception cref="ArgumentException">Thrown when any parameter does not match the expected size.</exception>
    public KeyPair(byte[] publicKey, byte[] publicHash, byte[] privateWithPassword, byte[] privateWithSeed,
        byte[] privateHash)
    {
        if (publicKey.Length != PublicKeySize)
            throw new ArgumentException($"Invalid Public Key size. Expected {PublicKeySize} bytes.");

        if (publicHash.Length != HashSize)
            throw new ArgumentException($"Invalid Public Hash size. Expected {HashSize} bytes.");

        if (privateWithPassword.Length != EncryptedPrivateKeySize)
            throw new ArgumentException($"Invalid PrivateWithPassword size. Expected {EncryptedPrivateKeySize} bytes.");

        if (privateWithSeed.Length != EncryptedPrivateKeySize)
            throw new ArgumentException($"Invalid PrivateWithSalt size. Expected {EncryptedPrivateKeySize} bytes.");

        if (privateHash.Length != HashSize)
            throw new ArgumentException($"Invalid Private Hash size. Expected {HashSize} bytes.");

        Public = publicKey;
        PublicHash = publicHash;
        PrivateWithPassword = privateWithPassword;
        PrivateWithSeed = privateWithSeed;
        PrivateHash = privateHash;
    }

    /// <summary>
    ///     Gets the identity's public key for encapsulation.
    /// </summary>
    public byte[] Public { get; init; }

    /// <summary>
    ///     Gets the cryptographic hash of the public key for indexing and verification.
    /// </summary>
    public byte[] PublicHash { get; init; }

    /// <summary>
    ///     Gets the private key wrapped with password-based protection.
    /// </summary>
    public byte[] PrivateWithPassword { get; init; }

    /// <summary>
    ///     Gets the private key wrapped with seed-based protection.
    /// </summary>
    public byte[] PrivateWithSeed { get; init; }

    /// <summary>
    ///     Gets the cryptographic hash of the private key material for integrity auditing.
    /// </summary>
    public byte[] PrivateHash { get; init; }
}