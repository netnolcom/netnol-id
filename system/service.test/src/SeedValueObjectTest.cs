using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Test;

public class SeedValueObjectTest
{
    private readonly byte[] _validEncryptedSeed = new byte[Seed.EncryptedSeedSize];
    private readonly byte[] _validHash = new byte[Seed.HashSize];

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Act
        var seed = new Seed(_validEncryptedSeed, _validHash);

        // Assert
        Assert.Equal(Seed.EncryptedSeedSize, (uint)seed.EncryptedValue.Length);
        Assert.Equal(Seed.HashSize, (uint)seed.Hash.Length);
        Assert.Equal(_validEncryptedSeed, seed.EncryptedValue);
        Assert.Equal(_validHash, seed.Hash);
    }

    [Theory]
    [InlineData(63)] // Invalid hash size
    [InlineData(65)]
    public void Constructor_WithInvalidHashSize_ShouldThrowArgumentException(int invalidSize)
    {
        // Arrange
        var invalidHash = new byte[invalidSize];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Seed(_validEncryptedSeed, invalidHash));

        Assert.Contains($"Expected {Seed.HashSize} bytes", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidEncryptedSize_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidEncrypted = new byte[10];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Seed(invalidEncrypted, _validHash));

        Assert.Contains($"Expected {Seed.EncryptedSeedSize} bytes", exception.Message);
    }
}