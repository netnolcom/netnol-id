using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Test;

public class IdentificationValueObjectTest
{
    [Fact]
    public void Constructor_WithValidValue_ShouldCreateInstance()
    {
        // Arrange
        var expected = UInt128.Parse("0123456789abcdef0123456789abcdef", System.Globalization.NumberStyles.HexNumber);

        // Act
        var id = new Identification(expected);

        // Assert
        Assert.Equal(expected, id.Value);
    }

    [Fact]
    public void Constructor_WithZero_ShouldThrowArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Identification(UInt128.Zero));

        Assert.Contains("cannot be zero", exception.Message);
    }

    [Fact]
    public void ToString_ShouldReturnCorrectHexFormat()
    {
        // Arrange
        var val = UInt128.MaxValue;
        var id = new Identification(val);

        // Act
        var result = id.ToString();

        // Assert
        Assert.Equal(32, result.Length);
        Assert.Equal("ffffffffffffffffffffffffffffffff", result);
    }
}