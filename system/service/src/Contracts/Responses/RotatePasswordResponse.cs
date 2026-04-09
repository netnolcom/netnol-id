using Netnol.Identity.Core;
using Netnol.Identity.Service.Application.Contracts.Outputs;

namespace Netnol.Identity.Service.Contracts.Responses;

public record RotatePasswordResponse(string Id, string Username, string PublicKey)
{
    public static RotatePasswordResponse FromOutput(RotatePasswordOutput output)
    {
        return new RotatePasswordResponse(output.Id.ToString(), output.Username.Value,
            CONVERTER.FromBinary(output.PublicKey));
    }
}