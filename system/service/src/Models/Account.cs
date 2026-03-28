namespace Netnol.Identity.Service.Models;

using Netnol.Identity.Service.Models.ObjectValues;

/// <summary>
/// Represents the root aggregate for a network account, orchestrating
/// identity, credentials, and cryptographic keys.
/// </summary>
public class Account
{
    public Identification Id { get; init; }
    public Username Username { get; init; }
    public KeyPair Keys { get; init; }
    public Seed Seed { get; init; }
    public Password Password { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Account"/> class.
    /// </summary>
    public Account(Identification id, Username username, KeyPair keys, Seed seed, Password password)
    {
        Id = id;
        Username = username;
        Keys = keys;
        Seed = seed;
        Password = password;
    }
}