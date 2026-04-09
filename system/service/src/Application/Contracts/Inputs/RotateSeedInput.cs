namespace Netnol.Identity.Service.Application.Contracts.Inputs;

public record RotateSeedInput(
    string Username,
    string OldSeedHash,
    string NewSeedHash,
    string EncryptedPrivateKeyWithNewSeed,
    string EncryptedSeedWithMasterKey);