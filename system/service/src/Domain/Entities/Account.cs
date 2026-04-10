using Netnol.Identity.Service.Domain.ValueObjects;
using StringQueryMap;

namespace Netnol.Identity.Service.Domain.Entities;

/// <summary>
///     Represents the root aggregate for a network account, orchestrating
///     identity, credentials, and cryptographic keys.
/// </summary>
public class Account()
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Account" /> class.
    /// </summary>
    /// <param name="id">The unique machine-level identifier.</param>
    /// <param name="username">The network identity name and hash.</param>
    /// <param name="keys">The cryptographic key pair with protected private keys.</param>
    /// <param name="seed">The encrypted master seed and integrity hash.</param>
    /// <param name="password">The password-based credential with derivation parameters.</param>
    public Account(Identification id, Username username, KeyPair keys, Seed seed, Password password) : this()
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
    public Identification Id { get; set; }

    /// <summary>
    ///     Gets the network identity name (username) and its cryptographic hash.
    /// </summary>
    public Username Username { get; set; }

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

        Keys = new KeyPair(
            Keys.Public,
            Keys.PublicHash,
            privateHash: Keys.PrivateHash,
            privateWithPassword: Keys.PrivateWithPassword,
            privateWithSeed: encryptedPrivateKeyWithNewSeed
        );
    }

    public void UpdatePassword(byte[] encryptedPrivateKeyWithNewPassword, byte[] newPasswordHash,
        byte[] newPasswordSalt,
        uint passwordParallelismCost,
        uint passwordIterationCost, uint passwordMemoryCost)
    {
        Password = new Password(
            newPasswordHash,
            iteration: passwordIterationCost,
            parallelism: passwordParallelismCost,
            memory: passwordMemoryCost,
            salt: newPasswordSalt);

        Keys = new KeyPair(
            Keys.Public,
            Keys.PublicHash,
            privateHash: Keys.PrivateHash,
            privateWithPassword: encryptedPrivateKeyWithNewPassword,
            privateWithSeed: Keys.PrivateWithSeed
        );
    }

    public override string ToString()
    {
        var map = new SQM("=", ";");

        map.Add("AID", Id.ToString());
        map.Add("USR", Username.ToString());
        map.Add("KYS", Keys.ToString());
        map.Add("SED", Seed.ToString());
        map.Add("PWD", Password.ToString());

        return map.ToString();
    }

    public static Account Parse(string value)
    {
        var map = SQM.Parse(value, "=", ";");

        return new Account(
            id: Identification.Parse(map.Get<string>("AID")),
            username: Username.Parse(map.Get<string>("USR")),
            keys: KeyPair.Parse(map.Get<string>("KYS")),
            seed: Seed.Parse(map.Get<string>("SED")),
            password: Password.Parse(map.Get<string>("PWD"))
        );
    }
}