using Netnol.Identity.Core;
using Netnol.Identity.Service.Application.Contracts.Outputs;

namespace Netnol.Identity.Service.Contracts.Responses;

/// <summary>
///     Represents the public identity information shared with other participants.
/// </summary>
/// <param name="Id">The unique object identifier.</param>
/// <param name="Username">The public network identity name.</param>
/// <param name="PublicKey">The public key material.</param>
public record ProfileResponse(string Id, string Username, string PublicKey)
{
    public static ProfileResponse FromOutput(ProfileOutput output)
    {
        return new ProfileResponse(output.Id.ToString(), output.Username.Value, CONVERTER.FromBinary(output.PublicKey));
    }
}