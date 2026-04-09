namespace Netnol.Identity.Service.Contracts.Requests;

/// <summary>
///     Represents a request to update the password-based credential protection.
/// </summary>
/// <param name="NewPasswordHash">The new verification hash.</param>
/// <param name="NewPasswordSalt">The unique derivation salt.</param>
/// <param name="NewPasswordIterationCost">The computational cost parameter.</param>
/// <param name="NewPasswordMemoryCost">The memory cost parameter in MiB.</param>
/// <param name="NewPasswordParallelismCost">The concurrency cost parameter.</param>
/// <param name="EncryptedPrivateKeyWithNewPassword">The private key material encrypted with the new password-derived key.</param>
public record RotatePasswordRequest(
    string OldPasswordHash,
    string NewPasswordHash,
    string NewPasswordSalt,
    uint NewPasswordIterationCost,
    uint NewPasswordMemoryCost,
    uint NewPasswordParallelismCost,
    string EncryptedPrivateKeyWithNewPassword);