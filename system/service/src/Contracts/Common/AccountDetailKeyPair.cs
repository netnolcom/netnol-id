namespace Netnol.Identity.Service.Contracts.Common;

/// <summary>
///     Defines the identity's key pair and the secure wrappers for private material.
/// </summary>
/// <param name="PublicKey">The public key material.</param>
/// <param name="PublicKeyHash">The integrity digest of the public key.</param>
/// <param name="PrivateKeyHash">The integrity digest of the private key.</param>
/// <param name="PrivateKeyWithSeed">The private key material protected by the master seed.</param>
/// <param name="PrivateKeyWithPassword">The private key material protected by the user password.</param>
public record AccountDetailKeyPair(
    string PublicKey,
    string PublicKeyHash,
    string PrivateKeyHash,
    string PrivateKeyWithSeed,
    string PrivateKeyWithPassword);