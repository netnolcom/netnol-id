using Netnol.Identity.Core;

namespace Netnol.Identity.Service.Models.ObjectValues;

/// <summary>
///     Represents the identity's cryptographic key pair, including identification hashes
///     and protected private keys for different security contexts.
/// </summary>
public readonly record struct KeyPair
{
    public const uint PublicKeySize = KEM.ExpectedPublicKeyLength;
    public const uint PrivateKeySize = KEM.ExpectedPrivateKeyLength;
    public const uint EncryptedPrivateKeySize = PrivateKeySize + CIPHER.ExpectedTagSize + CIPHER.ExpectedNonceSize;
    public const uint HashSize = HASH.ExpectedCompute512Size;

    /// <summary>
    ///     Initializes a new <see cref="KeyPair" /> with strict size validation for keys and hashes.
    /// </summary>
    public KeyPair(byte[] publicKey, byte[] publicHash, byte[] privateWithPassword, byte[] privateWithSalt,
        byte[] privateHash)
    {
        if (publicKey.Length != PublicKeySize)
            throw new ArgumentException($"Invalid Public Key size. Expected {PublicKeySize} bytes.");

        if (publicHash.Length != HashSize)
            throw new ArgumentException($"Invalid Public Hash size. Expected {HashSize} bytes.");

        if (privateWithPassword.Length != EncryptedPrivateKeySize)
            throw new ArgumentException($"Invalid PrivateWithPassword size. Expected {EncryptedPrivateKeySize} bytes.");

        if (privateWithSalt.Length != EncryptedPrivateKeySize)
            throw new ArgumentException($"Invalid PrivateWithSalt size. Expected {EncryptedPrivateKeySize} bytes.");

        if (privateHash.Length != HashSize)
            throw new ArgumentException($"Invalid Private Hash size. Expected {HashSize} bytes.");

        Public = publicKey;
        PublicHash = publicHash;
        PrivateWithPassword = privateWithPassword;
        PrivateWithSalt = privateWithSalt;
        PrivateHash = privateHash;
    }

    /// <summary>The identity's public key for encapsulation.</summary>
    public byte[] Public { get; }

    /// <summary>The cryptographic hash of the public key for indexing and verification.</summary>
    public byte[] PublicHash { get; }

    /// <summary>The private key wrapped with password-based protection.</summary>
    public byte[] PrivateWithPassword { get; }

    /// <summary>The private key wrapped with salt-based protection.</summary>
    public byte[] PrivateWithSalt { get; }

    /// <summary>The cryptographic hash of the private key material for integrity auditing.</summary>
    public byte[] PrivateHash { get; }
}