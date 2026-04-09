using Netnol.Identity.Core;
using StringQueryMap;

namespace Netnol.Identity.Service.Domain.ValueObjects;

/// <summary>
///     Represents the identity's master entropy, stored encrypted and protected by a 512-bit integrity hash.
/// </summary>
public readonly record struct Seed
{
    /// <summary>Raw master seed entropy size in bytes.</summary>
    public const uint SeedSize = 32;

    /// <summary>
    ///     Size of the seed when encrypted: raw seed + cipher tag + cipher nonce.
    /// </summary>
    public const uint EncryptedSeedSize = SeedSize + CIPHER.ExpectedTagSize + CIPHER.ExpectedNonceSize;

    /// <summary>Expected size of the integrity hash (SHA‑512) in bytes.</summary>
    public const uint HashSize = HASH.ExpectedCompute512Size;

    /// <summary>
    ///     Initializes a new <see cref="Seed" /> with strict size validation for encrypted data and hash.
    /// </summary>
    /// <param name="encryptedValue">The encrypted buffer (seed + tag + nonce).</param>
    /// <param name="hash">The 64‑byte integrity hash.</param>
    /// <exception cref="ArgumentException">Thrown when the encrypted value or hash size is incorrect.</exception>
    public Seed(byte[] encryptedValue, byte[] hash)
    {
        if (encryptedValue.Length != (int)EncryptedSeedSize)
            throw new ArgumentException($"Invalid Encrypted Seed size. Expected {EncryptedSeedSize} bytes.");

        if (hash.Length != (int)HashSize)
            throw new ArgumentException($"Invalid Seed Hash size. Expected {HashSize} bytes.");

        EncryptedValue = encryptedValue;
        Hash = hash;
    }

    /// <summary>
    ///     Gets the encrypted master seed entropy.
    /// </summary>
    public byte[] EncryptedValue { get; }

    /// <summary>
    ///     Gets the cryptographic hash for integrity and verification of the seed data.
    /// </summary>
    public byte[] Hash { get; }


    public override string ToString()
    {
        var map = new SQM("=", ";");

        map.Add("SDH", Hash);
        map.Add("ESM", EncryptedValue);

        return map.ToString();
    }

    public static Seed Parse(string value)
    {
        var map = SQM.Parse(value, "=", ";");

        return new Seed(map.Get<byte[]>("ESM"), map.Get<byte[]>("SDH"));
    }
}