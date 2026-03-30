using Netnol.Identity.Service.Contracts.Common;

namespace Netnol.Identity.Service.Contracts.Responses;

/// <summary>
///     Represents the comprehensive data profile of an identity.
/// </summary>
/// <param name="Oid">The internal unique object identifier.</param>
/// <param name="Nid">The public network identity name.</param>
/// <param name="Keys">The identity's key material and secure envelopes.</param>
/// <param name="Password">The password-based security profile.</param>
/// <param name="Seed">The seed-based security profile.</param>
public record AccountDetailResponse(
    UInt128 Oid,
    string Nid,
    AccountDetailKeyPair Keys,
    AccountDetailPassword Password,
    AccountDetailSeed Seed);