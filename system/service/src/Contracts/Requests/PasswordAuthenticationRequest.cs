namespace Netnol.Identity.Service.Contracts.Requests;

/// <summary>
///     Represents a verification proof for password-based authentication.
/// </summary>
/// <param name="Hash">The hexadecimal proof of password knowledge.</param>
public record PasswordAuthenticationRequest(string Hash);