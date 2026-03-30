namespace Netnol.Identity.Service.Contracts.Common;

/// <summary>
///     Defines the security parameters and verification data for password-based protection.
/// </summary>
/// <param name="Salt">The unique derivation salt.</param>
/// <param name="Iterations">The computational cost parameter.</param>
/// <param name="Memory">The memory cost parameter in MiB.</param>
/// <param name="Parallelism">The concurrency cost parameter.</param>
public record AccountDetailPassword(
    string Salt,
    uint Iterations,
    uint Memory,
    uint Parallelism);