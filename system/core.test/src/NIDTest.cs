namespace Netnol.Identity.Core.Test;

public class NIDTest
{
    [Theory]
    [InlineData("alice")]
    [InlineData("bob.123")]
    [InlineData("2026-netnol")]
    [InlineData("123")]
    [InlineData("admin.123-root")]
    public void IsValid_ShouldAcceptValidFormats(string input)
    {
        Assert.True(NID.IsValid(input));
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("this.name.is.way.too.long.to.be.valid")]
    [InlineData(".admin")]
    [InlineData("-user")]
    [InlineData("user.")]
    [InlineData("admin-")]
    [InlineData("james--bond")]
    [InlineData("dot..dot")]
    [InlineData("mix.-symbol")]
    [InlineData("other-.symbol")]
    [InlineData("user_name")]
    [InlineData("hello world")]
    [InlineData("special!char")]
    [InlineData("")]
    [InlineData(null)]
    public void IsValid_ShouldRejectInvalidFormats(string input)
    {
        Assert.False(NID.IsValid(input!));
    }

    [Fact]
    public void Normalize_ShouldCleanInput()
    {
        Assert.Equal("valid.user", NID.Normalize("  VALID.user  "));
        Assert.Equal("123.abc", NID.Normalize("123.ABC"));
        Assert.Equal(string.Empty, NID.Normalize(null!));
        Assert.Equal(string.Empty, NID.Normalize("   "));
    }

    [Fact]
    public void IsValid_ShouldWorkWithNormalizationInternally()
    {
        Assert.True(NID.IsValid("  Alice.123  "));
        Assert.True(NID.IsValid("BOB.B-42"));
    }

    [Fact]
    public void IsValid_BoundaryConditions()
    {
        Assert.True(NID.IsValid("a1b"));

        var maxName = new string('a', 30);
        Assert.True(NID.IsValid(maxName));

        var overMax = new string('a', 31);
        Assert.False(NID.IsValid(overMax));
    }
}