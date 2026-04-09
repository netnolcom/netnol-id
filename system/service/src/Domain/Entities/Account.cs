using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Domain.Entities;

/// <summary>
///     Represents the root aggregate for a network account, orchestrating
///     identity, credentials, and cryptographic keys.
/// </summary>
public class Account
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Account" /> class.
    /// </summary>
    /// <param name="id">The unique machine-level identifier.</param>
    /// <param name="username">The network identity name and hash.</param>
    /// <param name="keys">The cryptographic key pair with protected private keys.</param>
    /// <param name="seed">The encrypted master seed and integrity hash.</param>
    /// <param name="password">The password-based credential with derivation parameters.</param>
    public Account(Identification id, Username username, KeyPair keys, Seed seed, Password password)
    {
        Id = id;
        Username = username;
        Keys = keys;
        Seed = seed;
        Password = password;
    }

    /// <summary>
    ///     Gets the unique machine-level identifier for the account.
    /// </summary>
    public Identification Id { get; init; }

    /// <summary>
    ///     Gets the network identity name (username) and its cryptographic hash.
    /// </summary>
    public Username Username { get; init; }

    /// <summary>
    ///     Gets the cryptographic key pair with protected private keys.
    /// </summary>
    public KeyPair Keys { get; set; }

    /// <summary>
    ///     Gets the encrypted master seed and its integrity hash.
    /// </summary>
    public Seed Seed { get; set; }

    /// <summary>
    ///     Gets the password-based credential with derivation parameters.
    /// </summary>
    public Password Password { get; set; }

    public void UpdateSeed(byte[] encryptedSeedWithMasterKey, byte[] encryptedPrivateKeyWithNewSeed, byte[] newSeedHash)
    {
        Seed = new Seed(encryptedSeedWithMasterKey, newSeedHash);
        Keys = Keys with
        {
            PrivateWithSeed = encryptedPrivateKeyWithNewSeed
        };
    }

    public void UpdatePassword(byte[] encryptedPrivateKeyWithNewPassword, byte[] newPasswordHash, byte[] newPasswordSalt,
        uint passwordParallelismCost,
        uint passwordIterationCost, uint passwordMemoryCost)
    {
        Password = new Password(
            hash: newPasswordHash,
            iteration: passwordIterationCost,
            parallelism: passwordParallelismCost,
            memory: passwordMemoryCost,
            salt: newPasswordSalt);

        Keys = Keys with
        {
            PrivateWithPassword = encryptedPrivateKeyWithNewPassword
        };
    }
}