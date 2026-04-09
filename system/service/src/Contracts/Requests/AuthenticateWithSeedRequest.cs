namespace Netnol.Identity.Service.Contracts.Requests;

/// <summary>
///     Represents a verification proof for seed-based authentication.
/// </summary>
/// <param name="SeedHash">The hexadecimal proof of seed knowledge.</param>
public record AuthenticateWithSeedRequest(string SeedHash);