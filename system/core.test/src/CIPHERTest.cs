using System.Security.Cryptography;

namespace Netnol.Identity.Core.Test;

public class CIPHERTest
{
    [Fact]
    public void Protect_Unprotect_RoundTrip_ShouldMaintainIntegrity()
    {
        var originalData = RandomNumberGenerator.GetBytes(1024);
        var key = RandomNumberGenerator.GetBytes(32);

        var capsule = CIPHER.Protect(originalData, key);
        var decryptedData = CIPHER.Unprotect(capsule, key);

        Assert.Equal(originalData, decryptedData);
    }

    [Fact]
    public void Protect_ShouldProduceDifferentCapsules_ForSameDataAndKey()
    {
        var data = "ConsistentData"u8.ToArray();
        var key = RandomNumberGenerator.GetBytes(32);

        var capsule1 = CIPHER.Protect(data, key);
        var capsule2 = CIPHER.Protect(data, key);

        Assert.NotEqual(capsule1, capsule2);
    }

    [Fact]
    public void Protect_CapsuleSize_ShouldBeExact()
    {
        var dataSize = 100;
        var data = new byte[dataSize];
        var key = new byte[32];

        var capsule = CIPHER.Protect(data, key);

        var expectedSize = 12 + 16 + dataSize;
        Assert.Equal(expectedSize, capsule.Length);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(1024 * 1024)]
    public void Protect_Unprotect_VariousPayloadSizes(int size)
    {
        var data = new byte[size];
        var key = RandomNumberGenerator.GetBytes(32);
        RandomNumberGenerator.Fill(data);

        var capsule = CIPHER.Protect(data, key);
        var result = CIPHER.Unprotect(capsule, key);

        Assert.Equal(data, result);
    }

    [Fact]
    public void Unprotect_ShouldThrow_WhenNonceIsModified()
    {
        var data = new byte[32];
        var key = RandomNumberGenerator.GetBytes(32);
        var capsule = CIPHER.Protect(data, key);

        capsule[0] ^= 0x01;

        Assert.ThrowsAny<CryptographicException>(() => CIPHER.Unprotect(capsule, key));
    }

    [Fact]
    public void Unprotect_ShouldThrow_WhenTagIsModified()
    {
        var data = new byte[32];
        var key = RandomNumberGenerator.GetBytes(32);
        var capsule = CIPHER.Protect(data, key);

        capsule[12] ^= 0x01;

        Assert.ThrowsAny<CryptographicException>(() => CIPHER.Unprotect(capsule, key));
    }

    [Fact]
    public void Unprotect_ShouldThrow_WhenCiphertextIsModified()
    {
        var data = new byte[32];
        var key = RandomNumberGenerator.GetBytes(32);
        var capsule = CIPHER.Protect(data, key);

        capsule[28] ^= 0x01;

        Assert.ThrowsAny<CryptographicException>(() => CIPHER.Unprotect(capsule, key));
    }

    [Fact]
    public void Unprotect_ShouldThrow_WhenKeyIsWrong()
    {
        var data = new byte[32];
        var key = RandomNumberGenerator.GetBytes(32);
        var wrongKey = RandomNumberGenerator.GetBytes(32);

        var capsule = CIPHER.Protect(data, key);

        Assert.ThrowsAny<CryptographicException>(() => CIPHER.Unprotect(capsule, wrongKey));
    }

    [Fact]
    public void Protect_ShouldThrow_WhenKeyIsInvalidSize()
    {
        var data = new byte[16];
        var shortKey = new byte[16];

        Assert.Throws<ArgumentException>(() => CIPHER.Protect(data, shortKey));
    }

    [Fact]
    public void Unprotect_ShouldThrow_WhenCapsuleIsTooShort()
    {
        var key = new byte[32];
        var shortCapsule = new byte[27];

        Assert.Throws<ArgumentException>(() => CIPHER.Unprotect(shortCapsule, key));
    }

    [Fact]
    public void Protect_Unprotect_ShouldHandleNullInputs()
    {
        var key = new byte[32];
        var data = new byte[16];

        Assert.Throws<ArgumentNullException>(() => CIPHER.Protect(null!, key));
        Assert.Throws<ArgumentNullException>(() => CIPHER.Protect(data, null!));
        Assert.Throws<ArgumentNullException>(() => CIPHER.Unprotect(null!, key));
    }
}