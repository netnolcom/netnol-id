namespace Netnol.Identity.Service.Contracts.Requests;

/// <summary>
///     Represents a request to update the master seed protection envelope.
/// </summary>
/// <param name="NewSeedHash">The new seed verification hash.</param>
/// <param name="EncryptedPrivateKeyWithNewSeed">The private key material encrypted with the seed-derived key.</param>
public record RotateSeedRequest(
    string OldSeedHash,
    string NewSeedHash,
    string EncryptedPrivateKeyWithNewSeed,
    string EncryptedSeedWithMasterKey);