using System.Security.Cryptography;

namespace Netnol.Identity.Core.Test;

public class CONVERTERTest
{
    [Fact]
    public void ToBinary_ShouldReturnLowercaseHexString()
    {
        byte[] input = [0x00, 0x1A, 0x2B, 0x3C, 0xFF];
        const string expected = "001a2b3cff";

        var result = CONVERTER.FromBinary(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void FromBinary_ShouldReturnByteArray()
    {
        const string input = "001a2b3cff";
        byte[] expected = [0x00, 0x1A, 0x2B, 0x3C, 0xFF];

        var result = CONVERTER.ToBinary(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void RoundTrip_ShouldPreserveDataIntegrity()
    {
        var original = new byte[32];
        RandomNumberGenerator.Fill(original);

        var encoded = CONVERTER.FromBinary(original);
        var decoded = CONVERTER.ToBinary(encoded);

        Assert.Equal(original, decoded);
    }

    [Fact]
    public void FromBinary_ShouldHandleUpperCaseInput()
    {
        const string input = "ABCDEF01";
        byte[] expected = [0xAB, 0xCD, 0xEF, 0x01];

        var result = CONVERTER.ToBinary(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToBinary_ShouldThrowArgumentNullException_WhenInputIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => CONVERTER.FromBinary(null!));
    }

    [Fact]
    public void FromBinary_ShouldThrowArgumentNullException_WhenInputIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => CONVERTER.ToBinary(null!));
    }

    [Fact]
    public void FromBinary_ShouldThrowFormatException_WhenInputIsNotValidHex()
    {
        var invalidHex = "G1H2I3";

        Assert.Throws<FormatException>(() => CONVERTER.ToBinary(invalidHex));
    }
}