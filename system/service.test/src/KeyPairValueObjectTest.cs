using Netnol.Identity.Service.Models.ObjectValues;

namespace Netnol.Identity.Service.Test;

public class KeyPairValueObjectTest
{
    private readonly byte[] _validPublic = new byte[KeyPair.PublicKeySize];
    private readonly byte[] _validPublicHash = new byte[KeyPair.HashSize];
    private readonly byte[] _validPrivPass = new byte[KeyPair.EncryptedPrivateKeySize];
    private readonly byte[] _validPrivSalt = new byte[KeyPair.EncryptedPrivateKeySize];
    private readonly byte[] _validPrivHash = new byte[KeyPair.HashSize];

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Act
        var keyPair = new KeyPair(
            _validPublic,
            _validPublicHash,
            _validPrivPass,
            _validPrivSalt,
            _validPrivHash);

        // Assert
        Assert.Equal(KeyPair.HashSize, (uint)keyPair.PublicHash.Length);
        Assert.Equal(KeyPair.HashSize, (uint)keyPair.PrivateHash.Length);
        Assert.Equal(_validPublicHash, keyPair.PublicHash);
        Assert.Equal(_validPrivHash, keyPair.PrivateHash);
    }

    [Theory]
    [InlineData(63)]
    [InlineData(65)]
    public void Constructor_WithInvalidHashSize_ShouldThrowArgumentException(int invalidSize)
    {
        // Arrange
        var invalidHash = new byte[invalidSize];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new KeyPair(
                _validPublic,
                invalidHash, // Invalid Public Hash
                _validPrivPass,
                _validPrivSalt,
                _validPrivHash));

        Assert.Contains($"Invalid Public Hash size. Expected {KeyPair.HashSize} bytes.", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidPrivateHashSize_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidHash = new byte[10];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new KeyPair(
                _validPublic,
                _validPublicHash,
                _validPrivPass,
                _validPrivSalt,
                invalidHash)); // Invalid Private Hash

        Assert.Contains("Invalid Private Hash size", exception.Message);
    }
}