using Netnol.Identity.Core;

namespace Netnol.Identity.Service.Models.ObjectValues;

/// <summary>
///     Represents the identity's master entropy, stored encrypted and protected by a 512-bit integrity hash.
/// </summary>
public readonly record struct Seed
{
    /// <summary>The raw master seed entropy size.</summary>
    public const uint SeedSize = 32;

    /// <summary>The size of the seed when encrypted (Seed + Cipher-Tag + Cipher-Nonce).</summary>
    public const uint EncryptedSeedSize = SeedSize + CIPHER.ExpectedTagSize + CIPHER.ExpectedNonceSize;

    /// <summary>The size of the integrity has.</summary>
    public const uint HashSize = HASH.ExpectedCompute512Size;

    /// <summary>
    ///     Initializes a new <see cref="Seed" /> with strict size validation for encrypted data and hash.
    /// </summary>
    /// <param name="encryptedValue">The encrypted buffer (32 bytes entropy + Tag + Nonce).</param>
    /// <param name="hash">The 64-byte integrity hash.</param>
    public Seed(byte[] encryptedValue, byte[] hash)
    {
        if (encryptedValue.Length != (int)EncryptedSeedSize)
            throw new ArgumentException($"Invalid Encrypted Seed size. Expected {EncryptedSeedSize} bytes.");

        if (hash.Length != (int)HashSize)
            throw new ArgumentException($"Invalid Seed Hash size. Expected {HashSize} bytes.");

        EncryptedValue = encryptedValue;
        Hash = hash;
    }

    /// <summary>The encrypted master seed entropy.</summary>
    public byte[] EncryptedValue { get; }

    /// <summary>The cryptographic hash for integrity and verification of the seed data.</summary>
    public byte[] Hash { get; }
}