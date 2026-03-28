using Netnol.Identity.Service.Models.ObjectValues;
using Xunit;

namespace Netnol.Identity.Service.Test;

public class UsernameValueObjectTest
{
    private const string ValidNid = "alice.netnol";
    private readonly byte[] _validHash = new byte[Username.HashSize];

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Act
        var username = new Username(ValidNid, _validHash);

        // Assert
        Assert.Equal("alice.netnol", username.Value);
        Assert.Equal(_validHash, username.Hash);
    }

    [Fact]
    public void Constructor_WithInvalidNidFormat_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidNid = "a@b";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Username(invalidNid, _validHash));

        Assert.Contains($"Invalid Username format: '{invalidNid}'", exception.Message);
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
            new Username(ValidNid, invalidHash));

        Assert.Equal($"Invalid Username Hash size. Expected {Username.HashSize} bytes.", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldNormalizeValue()
    {
        // Arrange
        var unnormalized = "Alice.Netnol  ";

        // Act
        var username = new Username(unnormalized, _validHash);

        // Assert
        Assert.Equal("alice.netnol", username.Value);
    }
}