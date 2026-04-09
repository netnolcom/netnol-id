namespace Netnol.Identity.Service.Contracts.Requests;

/// <summary>
///     Represents a verification proof for password-based authentication.
/// </summary>
/// <param name="PasswordHash">The hexadecimal proof of password knowledge.</param>
public record AuthenticateWithPasswordRequest(string PasswordHash);