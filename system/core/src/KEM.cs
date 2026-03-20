using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Kems;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Netnol.Identity.Core;

/// <summary>
///     Provides a Post-Quantum Cryptography (PQC) Key Encapsulation Mechanism using the NIST standardized ML-KEM
///     algorithm. Designed to securely establish a shared symmetric secret over untrusted networks, protecting against
///     both classical and quantum computing attacks.
/// </summary>
public class KEM
{
    private const int ExpectedPublicKeyLength = 1568;
    private const int ExpectedPrivateKeyLength = 3168;

    /// <summary>
    ///     Represents the target ML-KEM parameter set. ML-KEM-1024 is utilized to provide the maximum theoretical security
    ///     margin (Category 5).
    /// </summary>
    private static readonly MLKemParameters Algorithm = MLKemParameters.ml_kem_1024;

    /// <summary>
    ///     Gets the raw, encoded ML-KEM-1024 private key (fixed size of 3168 bytes). This key must remain highly secured and
    ///     is strictly used to decapsulate incoming ciphertexts.
    /// </summary>
    public byte[] PrivateKey => (byte[])field.Clone();

    /// <summary>
    ///     Gets the raw, encoded ML-KEM-1024 public key (fixed size of 1568 bytes). This key is safe to distribute and is used
    ///     by external parties to encapsulate a shared secret.
    /// </summary>
    public byte[] PublicKey => (byte[])field.Clone();

    private KEM(byte[] publicKey, byte[] privateKey)
    {
        if (publicKey is not { Length: ExpectedPublicKeyLength })
            throw new ArgumentException($"Invalid Public Key: {ExpectedPublicKeyLength} bytes required.");

        if (privateKey is not { Length: ExpectedPrivateKeyLength })
            throw new ArgumentException($"Invalid Private Key: {ExpectedPrivateKeyLength} bytes required.");

        if (!IsValidPair(publicKey, privateKey))
            throw new ArgumentException("Key pair mismatch detected.");

        PublicKey = publicKey;
        PrivateKey = privateKey;
    }

    /// <summary>
    ///     Generates a fresh, cryptographically secure ML-KEM-1024 key pair using a robust random number generator. Typically
    ///     invoked during the initial identity provisioning phase.
    /// </summary>
    /// <returns>A new <see cref="KEM" /> instance encapsulating both the generated public and private keys.</returns>
    /// <exception cref="ArgumentException">Thrown if the underlying cryptographic engine fails to generate the keys.</exception>
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
            throw new ArgumentException($"Failed to generate ML-KEM: {e.Message}", e);
        }
    }

    /// <summary>
    ///     Restores a <see cref="KEM" /> instance from previously persisted key material. Performs an internal cryptographic
    ///     validation to ensure the provided public and private keys form a valid mathematical pair.
    /// </summary>
    /// <param name="publicKey">The 1568-byte encoded public key.</param>
    /// <param name="privateKey">The 3168-byte encoded private key.</param>
    /// <returns>A fully restored and validated <see cref="KEM" /> instance.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if the provided key arrays are malformed or do not mathematically correspond to each other.
    /// </exception>
    public static KEM Restore(byte[]? publicKey, byte[]? privateKey)
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
            throw new ArgumentException($"Failed to restore ML-KEM: {e.Message}", e);
        }
    }

    /// <summary>
    ///     Acts on behalf of the Sender: Generates a high-entropy symmetric secret and encapsulates it against the Recipient's
    ///     public key.
    /// </summary>
    /// <param name="publicKey">The 1568-byte public key of the intended recipient.</param>
    /// <returns>
    ///     A tuple containing:
    ///     - <c>Secret</c>: The agreed 32-byte symmetric key (ready to be used in algorithms like AES-256-GCM).
    ///     - <c>Cipher</c>: The 1568-byte encapsulation payload that must be transmitted over the network to the recipient.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if the public key argument is absent.</exception>
    /// <exception cref="ArgumentException">Thrown if the public key length is invalid or the encapsulation process fails.</exception>
    public static (byte[] Secret, byte[] Cipher) Encapsulate(byte[]? publicKey)
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
            encapsulator.Init(publicKeyObject);

            var cipher = new byte[encapsulator.EncapsulationLength];

            var secret = new byte[encapsulator.SecretLength];

            encapsulator.Encapsulate(cipher, 0, cipher.Length, secret, 0, secret.Length);

            return (secret, cipher);
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Failed to encapsulate shared key: {e.Message}", e);
        }
    }

    /// <summary>
    ///     Acts on behalf of the Recipient: Decapsulates a received ciphertext using their private key to recover the agreed
    ///     symmetric secret.
    /// </summary>
    /// <param name="cipher">The 1568-byte encapsulation payload received from the sender.</param>
    /// <param name="privateKey">The Recipient's secure 3168-byte private key.</param>
    /// <returns>The recovered 32-byte symmetric secret, mathematically identical to the one generated by the Sender.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the private key argument is absent.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the ciphertext or private key lengths are invalid, or if the decapsulation process is compromised.
    /// </exception>
    public static byte[] Decapsulate(byte[] cipher, byte[] privateKey)
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
            decapsulator.Init(privateKeyObject);

            var secret = new byte[decapsulator.SecretLength];

            if (cipher.Length != decapsulator.EncapsulationLength)
                throw new ArgumentException($"Ciphertext must be {decapsulator.EncapsulationLength} bytes.");

            decapsulator.Decapsulate(cipher, 0, cipher.Length, secret, 0, secret.Length);

            return secret;
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Failed to decapsulate shared key: {e.Message}", e);
        }
    }

    /// <summary>
    ///     Validates if a public and private key pair are mathematically compatible.
    ///     Performs a cryptographic round-trip (Encapsulate/Decapsulate) to ensure integrity.
    /// </summary>
    /// <param name="publicKey">The 1568-byte encoded public key.</param>
    /// <param name="privateKey">The 3168-byte encoded private key.</param>
    /// <returns><c>true</c> if the keys form a valid ML-KEM-1024 pair; otherwise, <c>false</c>.</returns>
    public static bool IsValidPair(byte[]? publicKey, byte[]? privateKey)
    {
        try
        {
            if (publicKey is not { Length: ExpectedPublicKeyLength } ||
                privateKey is not { Length: ExpectedPrivateKeyLength })
                return false;

            var (originalSecret, cipher) = Encapsulate(publicKey);
            var recoveredSecret = Decapsulate(cipher, privateKey);

            return originalSecret.Length == recoveredSecret.Length && originalSecret.SequenceEqual(recoveredSecret);
        }
        catch
        {
            return false;
        }
    }
}