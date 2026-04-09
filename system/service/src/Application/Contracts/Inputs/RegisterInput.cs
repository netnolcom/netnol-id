using System.Security.Cryptography.X509Certificates;

namespace Netnol.Identity.Service.Application.Contracts.Inputs;

public record RegisterInput(
    string Username,
    string PublicKey,
    string PublicKeyHash,
    string PrivateKeyHash,
    string EncryptedPrivateKeyWithPassword,
    string EncryptedPrivateKeyWithSeed,
    string EncryptedSeedWithMasterKey,
    string SeedHash,
    string PasswordHash,
    string PasswordSalt,
    uint PasswordMemoryCost,
    uint PasswordParallelismCost,
    uint PasswordIterationCost);