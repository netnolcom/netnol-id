namespace Netnol.Identity.Service.Contracts.Requests;

/// <summary>
///     Represents a verification proof for seed-based authentication.
/// </summary>
/// <param name="Hash">The hexadecimal proof of seed knowledge.</param>
public record SeedAuthenticationRequest(string Hash);