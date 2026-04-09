using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Application.Contracts.Outputs;

public record AuthenticateWithPasswordOutput(Identification Id, Username Username, byte[] PublicKey, byte[] PublicKeyHash, byte[] EncryptedPrivateKeyWithPassword, byte[] PrivateKeyHash);