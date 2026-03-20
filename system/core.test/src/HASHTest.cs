using System.Security.Cryptography;
using System.Text;

namespace Netnol.Identity.Core.Test;

public class HASHTests
{
    private readonly byte[] _testData = "Netnol.Identity.Core.Standard.Verification.Vector"u8.ToArray();
    private readonly byte[] _testKey = "HighEntropySecretKeyForAuthentication"u8.ToArray();

    [Fact]
    public void Compute256_Result_MustMatch_SystemSHA256()
    {
        var expected = SHA256.HashData(_testData);
        var actual = HASH.Compute256(_testData);

        Assert.Equal(expected, actual);
        Assert.Equal(32, actual.Length);
    }

    [Fact]
    public void Compute512_Result_MustMatch_SystemSHA512()
    {
        var expected = SHA512.HashData(_testData);
        var actual = HASH.Compute512(_testData);

        Assert.Equal(expected, actual);
        Assert.Equal(64, actual.Length);
    }

    [Fact]
    public void Authenticate_Result_MustMatch_SystemHMACSHA256()
    {
        var expected = HMACSHA256.HashData(_testKey, _testData);
        var actual = HASH.Authenticate(_testData, _testKey);

        Assert.Equal(expected, actual);
        Assert.Equal(32, actual.Length);
    }

    [Fact]
    public void ConstantTimeAreEqual_MustMatch_SystemFixedTimeEquals()
    {
        var a = HASH.Compute256(_testData);
        var b = HASH.Compute256(_testData);
        var c = HASH.Compute256("DifferentData"u8.ToArray());

        Assert.Equal(CryptographicOperations.FixedTimeEquals(a, b), HASH.ConstantTimeAreEqual(a, b));
        Assert.Equal(CryptographicOperations.FixedTimeEquals(a, c), HASH.ConstantTimeAreEqual(a, c));
    }

    [Theory]
    [InlineData("")]
    [InlineData("SingleWord")]
    [InlineData("A very long sentence to test the buffer padding and block processing of the SHA algorithm.")]
    public void Compute256_ShouldBeConsistentWithSystem_AcrossDifferentInputSizes(string input)
    {
        var data = Encoding.UTF8.GetBytes(input);
        var expected = SHA256.HashData(data);
        var actual = HASH.Compute256(data);

        Assert.Equal(expected, actual);
    }
}