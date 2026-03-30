namespace Netnol.Identity.Service.Contracts.Common;

/// <summary>
///     Defines the verification data and encryption integration for the master seed.
/// </summary>
/// <param name="WithPrivateKey">The master seed material protected by the identity's private key.</param>
public record AccountDetailSeed(string WithPrivateKey);