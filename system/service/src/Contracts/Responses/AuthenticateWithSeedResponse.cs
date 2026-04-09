using Netnol.Identity.Core;
using Netnol.Identity.Service.Application.Contracts.Outputs;

namespace Netnol.Identity.Service.Contracts.Responses;

public record AuthenticateWithSeedResponse(
    string Id,
    string Username,
    string PublicKey,
    string PublicKeyHash,
    string PrivateKeyHash,
    string EncryptedSeedWithMasterKey,
    string EncryptedPrivateKeyWithSeed)
{
    public static AuthenticateWithSeedResponse FromOutput(AuthenticateWithSeedOutput output)
    {
        return new AuthenticateWithSeedResponse(
            Id: output.Id.ToString(),
            Username: output.Username.Value,
            PublicKey: CONVERTER.FromBinary(output.PublicKey),
            PublicKeyHash: CONVERTER.FromBinary(output.PublicKeyHash),
            PrivateKeyHash: CONVERTER.FromBinary(output.PrivateKeyHash),
            EncryptedSeedWithMasterKey: CONVERTER.FromBinary(output.EncryptedSeedWithMasterKey),
            EncryptedPrivateKeyWithSeed: CONVERTER.FromBinary(output.EncryptedPrivateKeyWithSeed)
        );
    }
}