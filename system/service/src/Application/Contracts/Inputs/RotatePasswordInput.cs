namespace Netnol.Identity.Service.Application.Contracts.Inputs;

public record RotatePasswordInput(
    string Username,
    string OldPasswordHash,
    string NewPasswordHash,
    string NewPasswordSalt,
    string EncryptedPrivateKeyWithNewPassword,
    uint NewPasswordMemoryCost,
    uint NewPasswordParallelizationCost,
    uint NewPasswordIterationCost);