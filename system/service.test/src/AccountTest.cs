using Netnol.Identity.Service.Domain.Entities;
using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Test;

public class AccountTest
{
    private static readonly UInt128 ValidOid = UInt128.Parse("0123456789abcdef0123456789abcdef", System.Globalization.NumberStyles.HexNumber);
    private const string ValidNid = "satoshi.netnol";

    // --- Helper Method to Fill Buffers with Unique Patterns ---
    // This strictly prevents "Cross-Contamination" during object instantiation.
    private byte[] CreateFilledBuffer(uint size, byte fillValue)
    {
        var buffer = new byte[size];
        Array.Fill(buffer, fillValue);
        return buffer;
    }

    [Fact]
    public void Constructor_ShouldMapEveryComponent_WithoutCrossContamination()
    {
        // 1. Arrange - Creating data payloads with unique byte signatures (0x11, 0x22, 0x33...)
        var id = new Identification(ValidOid);

        var username = new Username(ValidNid, CreateFilledBuffer(Username.HashSize, 0x11));

        var password = new Password(
            hash: CreateFilledBuffer(Password.HashSize, 0x22),
            salt: CreateFilledBuffer(Password.SaltSize, 0x33),
            iteration: 10, memory: 512, parallelism: 2);

        var keys = new KeyPair(
            publicKey: CreateFilledBuffer(KeyPair.PublicKeySize, 0x44),
            publicHash: CreateFilledBuffer(KeyPair.HashSize, 0x55),
            privateWithPassword: CreateFilledBuffer(KeyPair.EncryptedPrivateKeySize, 0x66),
            privateWithSalt: CreateFilledBuffer(KeyPair.EncryptedPrivateKeySize, 0x77),
            privateHash: CreateFilledBuffer(KeyPair.HashSize, 0x88));

        var seed = new Seed(
            encryptedValue: CreateFilledBuffer(Seed.EncryptedSeedSize, 0x99),
            hash: CreateFilledBuffer(Seed.HashSize, 0xAA));

        // 2. Act - Instantiate the Aggregate Root
        var account = new Account(id, username, keys, seed, password);

        // 3. Assert - Strict Content Validation (Ensures no buffers were swapped)
        Assert.Equal(ValidOid, account.Id.Value);
        Assert.Equal(ValidNid, account.Username.Value);

        // Network Identity Validation
        Assert.All(account.Username.Hash, b => Assert.Equal(0x11, b));

        // Password Validation
        Assert.All(account.Password.Hash, b => Assert.Equal(0x22, b));
        Assert.All(account.Password.Salt, b => Assert.Equal(0x33, b));

        // Key Vault Validation (PQC + AES Wrappers)
        Assert.All(account.Keys.Public, b => Assert.Equal(0x44, b));
        Assert.All(account.Keys.PublicHash, b => Assert.Equal(0x55, b));
        Assert.All(account.Keys.PrivateWithPassword, b => Assert.Equal(0x66, b));
        Assert.All(account.Keys.PrivateWithSalt, b => Assert.Equal(0x77, b));
        Assert.All(account.Keys.PrivateHash, b => Assert.Equal(0x88, b));

        // Master Seed Validation
        Assert.All(account.Seed.EncryptedValue, b => Assert.Equal(0x99, b));
        Assert.All(account.Seed.Hash, b => Assert.Equal(0xAA, b));
    }

    [Fact]
    public void Account_ShouldHoldReferences_ToOptimizeMemory()
    {
        // In PQC cryptography, keys are massive (e.g., ML-KEM is 3KB+).
        // The Aggregate must not create hidden copies of the arrays to avoid memory bloat (GC Pressure).
        // This test proves that the Account uses direct references.

        // Arrange
        var hugePublicKeyBuffer = new byte[KeyPair.PublicKeySize];
        var keys = new KeyPair(
            hugePublicKeyBuffer,
            new byte[KeyPair.HashSize],
            new byte[KeyPair.EncryptedPrivateKeySize],
            new byte[KeyPair.EncryptedPrivateKeySize],
            new byte[KeyPair.HashSize]);

        var account = new Account(
            new Identification(ValidOid),
            new Username(ValidNid, new byte[Username.HashSize]),
            keys,
            new Seed(new byte[Seed.EncryptedSeedSize], new byte[Seed.HashSize]),
            new Password(new byte[Password.HashSize], new byte[Password.SaltSize], 1, 32, 1));

        // Act - Intentionally modify the original buffer
        hugePublicKeyBuffer[0] = 0xFF;

        // Assert - The Account reflects the change, proving it holds the exact same memory pointer
        Assert.Equal(0xFF, account.Keys.Public[0]);
        Assert.Same(hugePublicKeyBuffer, account.Keys.Public);
    }
}