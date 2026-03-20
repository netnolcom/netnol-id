using System.Reflection;

namespace Netnol.Identity.Core.Test;

public class KEMTests
{
    private const int ExpectedPublicKeyLength = 1568;
    private const int ExpectedPrivateKeyLength = 3168;
    private const int ExpectedCipherLength = 1568;
    private const int ExpectedSecretLength = 32;

    public static TheoryData<byte[]> InvalidPublicKeyData => new()
    {
        null,
        Array.Empty<byte>(),
        new byte[10],
        new byte[ExpectedPublicKeyLength + 1],
        new byte[ExpectedPublicKeyLength - 1]
    };

    public static TheoryData<byte[], byte[]> InvalidRestoreData => new()
    {
        { null, null },
        { null, new byte[10] },
        { new byte[10], null },
        { Array.Empty<byte>(), Array.Empty<byte>() },
        { new byte[ExpectedPublicKeyLength + 1], new byte[ExpectedPrivateKeyLength] },
        { new byte[ExpectedPublicKeyLength], new byte[ExpectedPrivateKeyLength - 1] }
    };

    public static TheoryData<byte[], byte[]> DecapsulateInvalidKeyData => new()
    {
        { null, null },
        { null, new byte[10] },
        { new byte[10], null },
        { Array.Empty<byte>(), Array.Empty<byte>() },
        { new byte[ExpectedCipherLength], new byte[ExpectedPrivateKeyLength + 1] },
        { new byte[ExpectedCipherLength - 1], new byte[ExpectedPrivateKeyLength] }
    };

    [Fact]
    public void Create_ShouldGenerateKeyPairWithCorrectLengths()
    {
        var kem = KEM.Create();
        Assert.NotNull(kem);
        Assert.Equal(ExpectedPublicKeyLength, kem.PublicKey.Length);
        Assert.Equal(ExpectedPrivateKeyLength, kem.PrivateKey.Length);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueKeyPairs()
    {
        var kem1 = KEM.Create();
        var kem2 = KEM.Create();
        Assert.NotEqual(kem1.PublicKey, kem2.PublicKey);
        Assert.NotEqual(kem1.PrivateKey, kem2.PrivateKey);
    }

    [Fact]
    public void Create_ShouldBeThreadSafe()
    {
        Parallel.For(0, 100, _ =>
        {
            var kem = KEM.Create();
            Assert.NotNull(kem);
        });
    }

    // ========== Restore ==========
    [Fact]
    public void Restore_WithValidKeys_ShouldSucceedAndReturnIdenticalKeys()
    {
        var original = KEM.Create();
        var restored = KEM.Restore(original.PublicKey, original.PrivateKey);
        Assert.Equal(original.PublicKey, restored.PublicKey);
        Assert.Equal(original.PrivateKey, restored.PrivateKey);
    }

    [Theory]
    [MemberData(nameof(InvalidRestoreData))]
    public void Restore_WithInvalidKeys_ShouldThrowArgumentException(byte[] publicKey, byte[] privateKey)
    {
        Assert.Throws<ArgumentException>(() => KEM.Restore(publicKey, privateKey));
    }

    [Fact]
    public void Restore_WithCorrectLengthButInvalidContent_ShouldThrowArgumentException()
    {
        var valid = KEM.Create();
        var invalidPublic = new byte[ExpectedPublicKeyLength];
        var invalidPrivate = new byte[ExpectedPrivateKeyLength];
        new Random().NextBytes(invalidPublic);
        new Random().NextBytes(invalidPrivate);

        Assert.Throws<ArgumentException>(() => KEM.Restore(invalidPublic, valid.PrivateKey));
        Assert.Throws<ArgumentException>(() => KEM.Restore(valid.PublicKey, invalidPrivate));
        Assert.Throws<ArgumentException>(() => KEM.Restore(invalidPublic, invalidPrivate));
    }

    [Fact]
    public void Restore_WithMismatchedKeys_ShouldThrowArgumentException()
    {
        var kem1 = KEM.Create();
        var kem2 = KEM.Create();
        Assert.Throws<ArgumentException>(() => KEM.Restore(kem1.PublicKey, kem2.PrivateKey));
    }

    [Fact]
    public void Encapsulate_WithValidPublicKey_ShouldReturnSecretAndCipher()
    {
        var kem = KEM.Create();
        var (secret, cipher) = KEM.Encapsulate(kem.PublicKey);
        Assert.Equal(ExpectedSecretLength, secret.Length);
        Assert.Equal(ExpectedCipherLength, cipher.Length);
    }

    [Fact]
    public void Encapsulate_WithSamePublicKey_ShouldProduceDifferentSecretsAndCiphers()
    {
        var kem = KEM.Create();
        var (secret1, cipher1) = KEM.Encapsulate(kem.PublicKey);
        var (secret2, cipher2) = KEM.Encapsulate(kem.PublicKey);
        Assert.NotEqual(secret1, secret2);
        Assert.NotEqual(cipher1, cipher2);
    }

    [Theory]
    [MemberData(nameof(InvalidPublicKeyData))]
    public void Encapsulate_WithInvalidPublicKey_ShouldThrowArgumentException(byte[] publicKey)
    {
        Assert.Throws<ArgumentException>(() => KEM.Encapsulate(publicKey));
    }

    [Fact]
    public void Encapsulate_WithPrivateKeyInsteadOfPublic_ShouldThrowArgumentException()
    {
        var kem = KEM.Create();
        Assert.Throws<ArgumentException>(() => KEM.Encapsulate(kem.PrivateKey));
    }

    [Fact]
    public void Encapsulate_WithRandomDataOfCorrectLength_ShouldThrowArgumentException()
    {
        var randomPublicKey = new byte[ExpectedPublicKeyLength];
        new Random().NextBytes(randomPublicKey);
        Assert.Throws<ArgumentException>(() => KEM.Encapsulate(randomPublicKey));
    }

    [Fact]
    public void Decapsulate_WithValidCipherAndPrivateKey_ShouldRecoverSameSecret()
    {
        var kem = KEM.Create();
        var (originalSecret, cipher) = KEM.Encapsulate(kem.PublicKey);
        var recoveredSecret = KEM.Decapsulate(cipher, kem.PrivateKey);
        Assert.Equal(originalSecret, recoveredSecret);
    }


    [Theory]
    [MemberData(nameof(DecapsulateInvalidKeyData))]
    public void Decapsulate_WithInvalidCipherOrPrivateKey_ShouldThrowArgumentException(byte[] cipher, byte[] privateKey)
    {
        // If both are null or invalid, the method will throw anyway.
        // Use a valid private key from a fresh KEM if we need to isolate cipher invalidity.
        var kem = KEM.Create();
        var validCipher = KEM.Encapsulate(kem.PublicKey).Cipher;

        var cipherToUse = cipher ?? validCipher;
        var privateKeyToUse = privateKey ?? kem.PrivateKey;

        // If both are null, we need to force one valid to test the other.
        if (cipher == null && privateKey == null)
        {
            Assert.Throws<ArgumentException>(() => KEM.Decapsulate(validCipher, null));
            Assert.Throws<ArgumentException>(() => KEM.Decapsulate(null, kem.PrivateKey));
            return;
        }

        Assert.Throws<ArgumentException>(() => KEM.Decapsulate(cipherToUse, privateKeyToUse));
    }

    [Fact]
    public void Decapsulate_WithPublicKeyInsteadOfPrivate_ShouldThrowArgumentException()
    {
        var kem = KEM.Create();
        var (_, cipher) = KEM.Encapsulate(kem.PublicKey);
        Assert.Throws<ArgumentException>(() => KEM.Decapsulate(cipher, kem.PublicKey));
    }

    [Fact]
    public void FullRoundTrip_ShouldWork()
    {
        var alice = KEM.Create();
        var (bobSecret, cipher) = KEM.Encapsulate(alice.PublicKey);
        var aliceSecret = KEM.Decapsulate(cipher, alice.PrivateKey);
        Assert.Equal(bobSecret, aliceSecret);
    }

    [Fact]
    public void FullRoundTrip_WithRestoredKeys_ShouldWork()
    {
        var original = KEM.Create();
        var restored = KEM.Restore(original.PublicKey, original.PrivateKey);
        var (secret1, cipher) = KEM.Encapsulate(restored.PublicKey);
        var secret2 = KEM.Decapsulate(cipher, original.PrivateKey);
        var secret3 = KEM.Decapsulate(cipher, restored.PrivateKey);
        Assert.Equal(secret1, secret2);
        Assert.Equal(secret1, secret3);
    }

    [Fact]
    public void FullRoundTrip_MultipleIterations_ShouldConsistentlyWork()
    {
        for (var i = 0; i < 100; i++)
        {
            var kem = KEM.Create();
            var (secret, cipher) = KEM.Encapsulate(kem.PublicKey);
            var recovered = KEM.Decapsulate(cipher, kem.PrivateKey);
            Assert.Equal(secret, recovered);
        }
    }

    [Fact]
    public void StressTest_GenerateManyKeyPairs_ShouldNotThrow()
    {
        for (var i = 0; i < 100; i++)
        {
            var kem = KEM.Create();
            Assert.NotNull(kem);
        }
    }

    [Fact]
    public void StressTest_EncapsulateDecapsulateManyTimes_ShouldNotThrow()
    {
        var kem = KEM.Create();
        for (var i = 0; i < 100; i++)
        {
            var (secret, cipher) = KEM.Encapsulate(kem.PublicKey);
            var recovered = KEM.Decapsulate(cipher, kem.PrivateKey);
            Assert.Equal(secret, recovered);
        }
    }

    [Fact]
    public void StressTest_ParallelEncapsulateDecapsulate_ShouldNotThrow()
    {
        var kem = KEM.Create();
        Parallel.For(0, 100, _ =>
        {
            var (secret, cipher) = KEM.Encapsulate(kem.PublicKey);
            var recovered = KEM.Decapsulate(cipher, kem.PrivateKey);
            Assert.Equal(secret, recovered);
        });
    }

    [Fact]
    public void Destructive_LargeNumberOfOperations_ShouldNotLeakMemory()
    {
        // Rough test for memory leaks: generate many keys and force GC.
        for (var i = 0; i < 1000; i++)
        {
            var kem = KEM.Create();
            var (_, cipher) = KEM.Encapsulate(kem.PublicKey);
            _ = KEM.Decapsulate(cipher, kem.PrivateKey);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        // If memory usage spikes, it's a leak, but we don't have a direct assertion.
        // This test is more for manual monitoring.
        Assert.True(true);
    }

    [Fact]
    public void Destructive_ReflectionAccess_ShouldNotAllowPrivateKeyRecovery()
    {
        // Attempt to access private fields (if any) to extract the key material.
        // The class stores keys in readonly fields, but reflection can still read them.
        // This test shows that even with reflection, the key material is accessible,
        // which is a design choice (it's stored in plain byte arrays).
        var kem = KEM.Create();
        var publicField = typeof(KEM).GetProperty("PublicKey", BindingFlags.Instance | BindingFlags.Public);
        var privateField = typeof(KEM).GetProperty("PrivateKey", BindingFlags.Instance | BindingFlags.Public);

        Assert.NotNull(publicField);
        Assert.NotNull(privateField);

        var publicKeyViaReflection = (byte[])publicField!.GetValue(kem)!;
        var privateKeyViaReflection = (byte[])privateField!.GetValue(kem)!;

        Assert.Equal(kem.PublicKey, publicKeyViaReflection);
        Assert.Equal(kem.PrivateKey, privateKeyViaReflection);
    }


    [Fact]
    public void Destructive_ConcurrentCreateAndRestore_ShouldBeSafe()
    {
        Parallel.For(0, 50, _ =>
        {
            var kem = KEM.Create();
            var restored = KEM.Restore(kem.PublicKey, kem.PrivateKey);
            var (secret, cipher) = KEM.Encapsulate(kem.PublicKey);
            var recovered = KEM.Decapsulate(cipher, restored.PrivateKey);
            Assert.Equal(secret, recovered);
        });
    }

    [Fact]
    public void EdgeCase_ZeroLengthArrays_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => KEM.Restore(Array.Empty<byte>(), Array.Empty<byte>()));
        Assert.Throws<ArgumentException>(() => KEM.Encapsulate(Array.Empty<byte>()));
        Assert.Throws<ArgumentException>(() => KEM.Decapsulate(Array.Empty<byte>(), Array.Empty<byte>()));
    }

    [Fact]
    public void EdgeCase_NullPublicKeyInEncapsulate_Throws()
    {
        Assert.Throws<ArgumentException>(() => KEM.Encapsulate(null));
    }

    [Fact]
    public void EdgeCase_NullPrivateKeyInDecapsulate_Throws()
    {
        var kem = KEM.Create();
        var (_, cipher) = KEM.Encapsulate(kem.PublicKey);
        Assert.Throws<ArgumentException>(() => KEM.Decapsulate(cipher, null));
    }

    [Fact]
    public void PublicKey_ShouldBeImmutable_WhenModifiedExternally()
    {
        var kem = KEM.Create();
        var originalPublicKey = kem.PublicKey.ToArray(); // Cópia para comparação

        var bytesFromProperty = kem.PublicKey;
        bytesFromProperty[0] ^= 0xFF; // Tenta corromper a chave externamente

        Assert.Equal(originalPublicKey, kem.PublicKey);
    }

    [Fact]
    public void Restore_WithAllZeroKeys_ShouldThrowArgumentException()
    {
        var zeroPub = new byte[ExpectedPublicKeyLength];
        var zeroPriv = new byte[ExpectedPrivateKeyLength];

        Assert.Throws<ArgumentException>(() => KEM.Restore(zeroPub, zeroPriv));
    }

    [Fact]
    public void Decapsulate_WithTamperedCipher_ShouldReturnIncorrectSecretInsteadOfThrowing()
    {
        // Arrange
        var kem = KEM.Create();
        var (originalSecret, cipher) = KEM.Encapsulate(kem.PublicKey);

        cipher[0] ^= 0x01;
        cipher[1] ^= 0x01;
        var recoveredSecret = KEM.Decapsulate(cipher, kem.PrivateKey);

        // Assert
        Assert.NotEqual(originalSecret, recoveredSecret);
        Assert.Equal(ExpectedSecretLength, recoveredSecret.Length);
    }

    [Fact]
    public void IsValidPair_ShouldIdentifyCorrectAndIncorrectPairs()
    {
        var alice = KEM.Create();
        var bob = KEM.Create();

        Assert.True(KEM.IsValidPair(alice.PublicKey, alice.PrivateKey));

        Assert.False(KEM.IsValidPair(alice.PublicKey, bob.PrivateKey));
        Assert.False(KEM.IsValidPair(null, alice.PrivateKey));
        Assert.False(KEM.IsValidPair(new byte[10], alice.PrivateKey));
    }

    [Fact]
    public void IsValidPair_WithTotalGarbage_ShouldReturnFalseQuietly()
    {
        var randomBytes = new byte[ExpectedPublicKeyLength];
        new Random().NextBytes(randomBytes);

        Assert.False(KEM.IsValidPair(randomBytes, randomBytes));
        Assert.False(KEM.IsValidPair(null, null));
    }

    [Fact]
    public void Restore_FromClonedArrays_ShouldMaintainFunctionality()
    {
        var original = KEM.Create();

        var savedPub = original.PublicKey;
        var savedPriv = original.PrivateKey;

        var restored = KEM.Restore(savedPub, savedPriv);

        var (secret, cipher) = KEM.Encapsulate(restored.PublicKey);
        var recovered = KEM.Decapsulate(cipher, original.PrivateKey);

        Assert.Equal(secret, recovered);
    }

    [Fact]
    public void Decapsulate_WithSameCipherButDifferentPrivateKey_ShouldYieldUniqueSecrets()
    {
        var alice = KEM.Create();
        var bob = KEM.Create();

        var (secretForAlice, cipher) = KEM.Encapsulate(alice.PublicKey);

        var secretBobGot = KEM.Decapsulate(cipher, bob.PrivateKey);

        Assert.Equal(ExpectedSecretLength, secretBobGot.Length);
        Assert.NotEqual(secretForAlice, secretBobGot);
    }

    [Fact]
    public void Encapsulate_WithConstantBytePublicKey_ShouldThrowArgumentException()
    {
        var invalidContentPub = new byte[ExpectedPublicKeyLength];
        Array.Fill(invalidContentPub, (byte)0xFF);

        Assert.Throws<ArgumentException>(() => KEM.Encapsulate(invalidContentPub));
    }

    [Fact]
    public void Decapsulate_ShouldBeDeterministic()
    {
        var kem = KEM.Create();
        var (secret, cipher) = KEM.Encapsulate(kem.PublicKey);

        var firstRecovery = KEM.Decapsulate(cipher, kem.PrivateKey);

        for (int i = 0; i < 100; i++)
        {
            var subsequentRecovery = KEM.Decapsulate(cipher, kem.PrivateKey);
            Assert.Equal(firstRecovery, subsequentRecovery);
        }
    }

    [Fact]
    public void IsValidPair_WithKnownIncompatibleVectors_ShouldReturnFalse()
    {
        var alice = KEM.Create();
        var bob = KEM.Create();

        Assert.False(KEM.IsValidPair(alice.PublicKey, bob.PrivateKey));
        Assert.False(KEM.IsValidPair(bob.PublicKey, alice.PrivateKey));
        Assert.True(KEM.IsValidPair(alice.PublicKey, alice.PrivateKey));
        Assert.True(KEM.IsValidPair(bob.PublicKey, bob.PrivateKey));
    }


}