namespace Netnol.Identity.Service.Contracts.Requests;

/// <summary>
///     Represents a request to update the master seed protection envelope.
/// </summary>
/// <param name="Hash">The new seed verification hash.</param>
/// <param name="PrivateKeyWithSeed">The private key material encrypted with the seed-derived key.</param>
public record SeedRotationRequest(string Hash, string PrivateKeyWithSeed);