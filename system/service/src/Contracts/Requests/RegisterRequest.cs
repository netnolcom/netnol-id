using Netnol.Identity.Service.Contracts.Common;

namespace Netnol.Identity.Service.Contracts.Requests;

/// <summary>
///     Contains the complete identity and credential set required for account registration.
/// </summary>
/// <param name="Keys">The identity's cryptographic key material and protection wrappers.</param>
/// <param name="Password">The security parameters for password-based access.</param>
/// <param name="Seed">The entropy verification and master seed protection data.</param>
public record RegisterRequest(
    AccountDetailKeyPair Keys,
    AccountDetailPassword Password,
    AccountDetailSeed Seed);