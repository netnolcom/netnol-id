using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Application.Contracts.Outputs;

public record AuthenticateWithSeedOutput(Identification Id, Username Username, byte[] PublicKey, byte[] PublicKeyHash, byte[] EncryptedPrivateKeyWithSeed, byte[] PrivateKeyHash, byte[] EncryptedSeedWithMasterKey);