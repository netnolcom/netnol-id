using System.Security.Cryptography.X509Certificates;
using Netnol.Identity.Core;
using Netnol.Identity.Service.Application.Contracts.Outputs;

namespace Netnol.Identity.Service.Contracts.Responses;

public record AuthenticateWithPasswordResponse(
    string Id,
    string Username,
    string PublicKey,
    string PublicKeyHash,
    string EncryptedPrivateKeyWithPassword,
    string PrivateKeyHash)
{
    public static AuthenticateWithPasswordResponse FromOutput(AuthenticateWithPasswordOutput output)
    {
        return new AuthenticateWithPasswordResponse(
            Id: output.Id.ToString(),
            Username: output.Username.Value,
            PublicKey: CONVERTER.FromBinary(output.PublicKey),
            PublicKeyHash: CONVERTER.FromBinary(output.PublicKeyHash),
            EncryptedPrivateKeyWithPassword: CONVERTER.FromBinary(output.EncryptedPrivateKeyWithPassword),
            PrivateKeyHash: CONVERTER.FromBinary(output.PrivateKeyHash)
        );
    }
}