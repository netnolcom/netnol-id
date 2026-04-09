using Netnol.Identity.Core;
using Netnol.Identity.Service.Application.Contracts.Inputs;
using Netnol.Identity.Service.Application.Contracts.Outputs;

namespace Netnol.Identity.Service.Contracts.Responses;

public record RegisterResponse(string Id, string Username, string PublicKey)
{
    public static RegisterResponse FromOutput(RegisterOutput output)
    {
        return new RegisterResponse(output.Id.ToString(), output.Username.Value,
            CONVERTER.FromBinary(output.PublicKey));
    }
}