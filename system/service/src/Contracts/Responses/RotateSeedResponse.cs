using Netnol.Identity.Core;
using Netnol.Identity.Service.Application.Contracts.Outputs;

namespace Netnol.Identity.Service.Contracts.Responses;

public record RotateSeedResponse(string Id, string Username, string PublicKey)
{
    public static RotateSeedResponse FromOutput(RotateSeedOutput output)
    {
        return new RotateSeedResponse(output.Id.ToString(), output.Username.Value,
            CONVERTER.FromBinary(output.PublicKey));
    }
}