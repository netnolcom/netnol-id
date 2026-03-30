namespace Netnol.Identity.Service.Contracts.Responses;

/// <summary>
///     Represents the public identity information shared with other participants.
/// </summary>
/// <param name="Oid">The unique object identifier.</param>
/// <param name="Nid">The public network identity name.</param>
/// <param name="PublicKey">The public key material.</param>
public record ProfileResponse(UInt128 Oid, string Nid, string PublicKey);