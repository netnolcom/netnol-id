using Netnol.Identity.Service.Application.Common;
using Netnol.Identity.Service.Contracts.Requests;
using Netnol.Identity.Service.Contracts.Responses;

namespace Netnol.Identity.Service.Application.Services;

/// <summary>
///     Defines the application-level operations for account management and authentication.
/// </summary>
public interface IAccountService
{
    /// <summary>
    ///     Checks whether an account exists for the given network identity.
    /// </summary>
    /// <param name="nid">The network identity name.</param>
    /// <returns>A result containing true if the account exists, otherwise a failure with an appropriate error.</returns>
    Task<Result<bool>> CheckExistenceAsync(string nid);

    /// <summary>
    ///     Retrieves the public profile of an identity.
    /// </summary>
    /// <param name="nid">The network identity name.</param>
    /// <returns>A result containing the profile if found, otherwise a failure with ResourceNotFound.</returns>
    Task<Result<ProfileResponse>> GetProfileAsync(string nid);

    /// <summary>
    ///     Creates a new account with the provided credentials.
    /// </summary>
    /// <param name="nid">The network identity name.</param>
    /// <param name="request">The registration data.</param>
    /// <returns>A result containing the account details if successful, otherwise a failure with an appropriate error.</returns>
    Task<Result<AccountDetailResponse>> RegisterAsync(string nid, RegisterRequest request);

    /// <summary>
    ///     Generates a password authentication challenge for a given identity.
    /// </summary>
    /// <param name="nid">The network identity name.</param>
    /// <returns>
    ///     A result containing the challenge parameters if successful, otherwise a failure with ResourceNotFound or other
    ///     error.
    /// </returns>
    Task<Result<AuthenticationChallengeWithPasswordResponse>> GetPasswordChallengeAsync(string nid);

    /// <summary>
    ///     Authenticates using a password-based proof.
    /// </summary>
    /// <param name="nid">The network identity name.</param>
    /// <param name="request">The proof of knowledge.</param>
    /// <returns>
    ///     A result containing the account details if authentication succeeds, otherwise a failure with
    ///     AuthenticationRequired or other error.
    /// </returns>
    Task<Result<AccountDetailResponse>> AuthenticateWithPasswordAsync(string nid,
        PasswordAuthenticationRequest request);

    /// <summary>
    ///     Authenticates using a seed-based proof.
    /// </summary>
    /// <param name="nid">The network identity name.</param>
    /// <param name="request">The proof of knowledge.</param>
    /// <returns>
    ///     A result containing the account details if authentication succeeds, otherwise a failure with
    ///     AuthenticationRequired or other error.
    /// </returns>
    Task<Result<AccountDetailResponse>> AuthenticateWithSeedAsync(string nid, SeedAuthenticationRequest request);

    /// <summary>
    ///     Rotates the password-based credentials.
    /// </summary>
    /// <param name="nid">The network identity name.</param>
    /// <param name="request">The new password credentials.</param>
    /// <returns>A result containing the updated account details if successful, otherwise a failure with appropriate error.</returns>
    Task<Result<AccountDetailResponse>> RotatePasswordAsync(string nid, PasswordRotationRequest request);

    /// <summary>
    ///     Rotates the seed-based credentials.
    /// </summary>
    /// <param name="nid">The network identity name.</param>
    /// <param name="request">The new seed credentials.</param>
    /// <returns>A result containing the updated account details if successful, otherwise a failure with appropriate error.</returns>
    Task<Result<AccountDetailResponse>> RotateSeedAsync(string nid, SeedRotationRequest request);
}