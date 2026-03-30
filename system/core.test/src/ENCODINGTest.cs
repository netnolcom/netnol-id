using System.Text;

namespace Netnol.Identity.Core.Test;

public class ENCODINGTest
{
    [Fact]
    public void ToBinary_WithValidString_ReturnsCorrectBytes()
    {
        var input = "Hello World!";
        var expected = Encoding.UTF8.GetBytes(input);

        var result = ENCODING.ToBinary(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToBinary_WithEmptyString_ReturnsEmptyArray()
    {
        var result = ENCODING.ToBinary(string.Empty);

        Assert.Null(result);
    }

    [Fact]
    public void ToBinary_WithNullString_ReturnsEmptyArray()
    {
        var result = ENCODING.ToBinary(null!);

        Assert.Null(result);
    }

    [Fact]
    public void ToBinary_WithSpecialCharacters_ReturnsCorrectBytes()
    {
        var input = "Olá, 世界! 😊";
        var expected = Encoding.UTF8.GetBytes(input);

        var result = ENCODING.ToBinary(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToBinary_WithVeryLongString_DoesNotThrow()
    {
        var input = new string('A', 10_000);

        var result = ENCODING.ToBinary(input)!;

        Assert.Equal(10_000, result.Length);
    }

    [Fact]
    public void FromBinary_WithValidBytes_ReturnsCorrectString()
    {
        var input = Encoding.UTF8.GetBytes("Hello World!");
        var expected = "Hello World!";

        var result = ENCODING.FromBinary(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void FromBinary_WithEmptyArray_ReturnsEmptyString()
    {
        var result = ENCODING.FromBinary([]);

        Assert.Null(result);
    }

    [Fact]
    public void FromBinary_WithNullArray_ReturnsEmptyString()
    {
        var result = ENCODING.FromBinary(null!);

        Assert.Null(result);
    }

    [Fact]
    public void FromBinary_WithSpecialCharactersBytes_ReturnsCorrectString()
    {
        var input = Encoding.UTF8.GetBytes("Olá, 世界! 😊");
        var expected = "Olá, 世界! 😊";

        var result = ENCODING.FromBinary(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Roundtrip_AnyString_ReturnsOriginal()
    {
        string[] testStrings =
        [
            "",
            "Hello",
            "1234567890",
            "!@#$%^&*()",
            "Olá, mundo!",
            "🌍🌎🌏",
            new('A', 1000),
            "Café & 中文"
        ];

        foreach (var original in testStrings)
        {
            var bytes = ENCODING.ToBinary(original);
            var result = ENCODING.FromBinary(bytes!);

            Assert.Equal(string.IsNullOrEmpty(original) ? null : original, result);
        }
    }

    [Fact]
    public void Roundtrip_EmptyArray_ReturnsEmptyString()
    {
        var bytes = ENCODING.ToBinary(string.Empty);
        var result = ENCODING.FromBinary(bytes!);

        Assert.Null(result);
    }

    [Fact]
    public void ToBinary_LargeString_CompletesQuickly()
    {
        var large = new string('x', 1_000_000);

        var result = ENCODING.ToBinary(large)!;

        Assert.Equal(1_000_000, result.Length);
    }

    [Fact]
    public void FromBinary_LargeArray_CompletesQuickly()
    {
        var large = new byte[1_000_000];
        for (var i = 0; i < large.Length; i++) large[i] = (byte)'a';

        var result = ENCODING.FromBinary(large)!;

        Assert.Equal(1_000_000, result.Length);
    }
}