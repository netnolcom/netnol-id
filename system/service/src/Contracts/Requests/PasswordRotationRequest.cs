namespace Netnol.Identity.Service.Contracts.Requests;

/// <summary>
///     Represents a request to update the password-based credential protection.
/// </summary>
/// <param name="Hash">The new verification hash.</param>
/// <param name="Salt">The unique derivation salt.</param>
/// <param name="Iterations">The computational cost parameter.</param>
/// <param name="Memory">The memory cost parameter in MiB.</param>
/// <param name="Parallelism">The concurrency cost parameter.</param>
/// <param name="PrivateKeyWithPassword">The private key material encrypted with the new password-derived key.</param>
public record PasswordRotationRequest(
    string Hash,
    string Salt,
    uint Iterations,
    uint Memory,
    uint Parallelism,
    string PrivateKeyWithPassword);