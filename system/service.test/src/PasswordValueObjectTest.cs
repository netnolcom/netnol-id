using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Test;

public class PasswordValueObjectTest
{
    private readonly byte[] _validHash = new byte[Password.HashSize];
    private readonly byte[] _validSalt = new byte[Password.SaltSize];
    private const uint ValidIterations = 10;
    private const uint ValidMemory = 512;
    private const uint ValidParallelism = 2;

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Act
        var password = new Password(_validHash, _validSalt, ValidIterations, ValidMemory, ValidParallelism);

        // Assert
        Assert.Equal(_validHash, password.Hash);
        Assert.Equal(_validSalt, password.Salt);
        Assert.Equal(ValidIterations, password.Iterations);
        Assert.Equal(ValidMemory, password.Memory);
        Assert.Equal(ValidParallelism, password.Parallelism);
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
            new Password(invalidHash, _validSalt, ValidIterations, ValidMemory, ValidParallelism));

        Assert.Equal($"Invalid Hash size. Expected {Password.HashSize} bytes.", exception.Message);
    }

    [Theory]
    [InlineData(31)]
    [InlineData(33)]
    public void Constructor_WithInvalidSaltSize_ShouldThrowArgumentException(int invalidSize)
    {
        // Arrange
        var invalidSalt = new byte[invalidSize];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Password(_validHash, invalidSalt, ValidIterations, ValidMemory, ValidParallelism));

        Assert.Equal($"Invalid Salt size. Expected {Password.SaltSize} bytes.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Constructor_WithInvalidIterations_ShouldThrowArgumentException(uint invalidIterations)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Password(_validHash, _validSalt, invalidIterations, ValidMemory, ValidParallelism));

        Assert.Contains($"Iterations ({invalidIterations}) must be between", exception.Message);
    }

    [Theory]
    [InlineData(31)]
    [InlineData(2049)]
    public void Constructor_WithInvalidMemory_ShouldThrowArgumentException(uint invalidMemory)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Password(_validHash, _validSalt, ValidIterations, invalidMemory, ValidParallelism));

        Assert.Contains($"Memory ({invalidMemory}) must be between", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Constructor_WithInvalidParallelism_ShouldThrowArgumentException(uint invalidParallelism)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Password(_validHash, _validSalt, ValidIterations, ValidMemory, invalidParallelism));

        Assert.Contains($"Parallelism ({invalidParallelism}) must be between", exception.Message);
    }
}