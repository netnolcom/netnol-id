namespace Netnol.Identity.Core.Test;

public class OIDTest
{
    [Fact]
    public void New_ShouldBeSequential()
    {
        var id1 = OID.New();
        var id2 = OID.New();

        Assert.True(id2 >= id1);
    }

    [Fact]
    public void ParseAndToString_ShouldMatch()
    {
        var original = OID.New();

        var hex = OID.ToString(original);
        Assert.Equal(32, hex.Length);

        var restored = OID.Parse(hex);
        Assert.Equal(original, restored);
    }

    [Fact]
    public void ShouldHandleMaxValues()
    {
        var maxHex = new string('f', 32);
        var maxVal = OID.Parse(maxHex);

        Assert.Equal(UInt128.MaxValue, maxVal);
        Assert.Equal(maxHex, OID.ToString(maxVal));
    }

    [Fact]
    public void ShouldHandleMinValues()
    {
        var minHex = new string('0', 32);
        var minVal = OID.Parse(minHex);

        Assert.Equal(UInt128.MinValue, minVal);
        Assert.Equal(minHex, OID.ToString(minVal));
    }
}