using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Kems;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Netnol.Identity.Core;

public class KEM
{
    private const int ExpectedPublicKeyLength = 1568;
    private const int ExpectedPrivateKeyLength = 3168;

    private static readonly MLKemParameters Algorithm = MLKemParameters.ml_kem_1024;

    public readonly byte[] PublicKey;
    public readonly byte[] PrivateKey;

    private KEM(byte[] publicKey, byte[] privateKey)
    {
        if (publicKey is not { Length: ExpectedPublicKeyLength })
            throw new ArgumentException($"Invalid Public Key: {ExpectedPublicKeyLength} bytes required.");

        if (privateKey is not { Length: ExpectedPrivateKeyLength })
            throw new ArgumentException($"Invalid Private Key: {ExpectedPrivateKeyLength} bytes required.");

        PublicKey = publicKey;
        PrivateKey = privateKey;
    }

    public static KEM Create()
    {
        try
        {
            var generator = new MLKemKeyPairGenerator();
            generator.Init(new MLKemKeyGenerationParameters(new SecureRandom(), Algorithm));

            var keypair = generator.GenerateKeyPair();

            var publicKey = (MLKemPublicKeyParameters)keypair.Public;

            var privateKey = (MLKemPrivateKeyParameters)keypair.Private;

            return new KEM(publicKey.GetEncoded(), privateKey.GetEncoded());
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Failed to generate KEM: {e.Message}", e);
        }
    }

    public static KEM Create(byte[]? publicKey, byte[]? privateKey)
    {
        try
        {
            var publicKeyObject = MLKemPublicKeyParameters.FromEncoding(Algorithm, publicKey);

            var privateKeyObject = MLKemPrivateKeyParameters.FromEncoding(Algorithm, privateKey);

            _ = new AsymmetricCipherKeyPair(publicKeyObject, privateKeyObject); // validate keypair

            return new KEM(publicKeyObject.GetEncoded(), privateKeyObject.GetEncoded());
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Failed to restore KEM: {e.Message}", e);
        }
    }

    public static (byte[] Secret, byte[] Cipher) GenerateSharedKey(byte[]? publicKey)
    {
        try
        {
            if (publicKey is not { Length: > 0 })
                throw new ArgumentNullException(nameof(publicKey), "Public key cannot be null or empty.");

            if (publicKey is not { Length: ExpectedPublicKeyLength })
                throw new ArgumentException($"Invalid Public Key: {ExpectedPublicKeyLength} bytes required.");

            var publicKeyObject = MLKemPublicKeyParameters.FromEncoding(Algorithm, publicKey);

            if (publicKeyObject.IsPrivate)
                throw new InvalidKeyException("Public key expected, but private key provided.");

            var encapsulator = new MLKemEncapsulator(Algorithm);

            var cipher = new byte[encapsulator.EncapsulationLength];

            var secret = new byte[encapsulator.SecretLength];

            encapsulator.Init(publicKeyObject);
            encapsulator.Encapsulate(cipher, 0, cipher.Length, secret, 0, secret.Length);

            return (secret, cipher);
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Failed to encapsulate shared key: {e.Message}", e);
        }
    }

    public static byte[] RestoreSharedKey(byte[] cipher, byte[] privateKey)
    {
        try
        {
            if (privateKey is not { Length: > 0 })
                throw new ArgumentNullException(nameof(privateKey), "Private key cannot be null or empty.");

            if (privateKey is not { Length: ExpectedPrivateKeyLength })
                throw new ArgumentException($"Invalid Private Key: {ExpectedPrivateKeyLength} bytes required.");

            var privateKeyObject = MLKemPrivateKeyParameters.FromEncoding(Algorithm, privateKey);

            if (!privateKeyObject.IsPrivate)
                throw new InvalidKeyException("Private key expected, but public key provided.");

            var decapsulator = new MLKemDecapsulator(Algorithm);

            var secret = new byte[decapsulator.SecretLength];

            if (cipher.Length != decapsulator.EncapsulationLength)
                throw new ArgumentException($"Ciphertext must be {decapsulator.EncapsulationLength} bytes.");

            decapsulator.Init(privateKeyObject);
            decapsulator.Decapsulate(cipher, 0, cipher.Length, secret, 0, secret.Length);

            return secret;
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Failed to decapsulate shared key: {e.Message}", e);
        }
    }
}