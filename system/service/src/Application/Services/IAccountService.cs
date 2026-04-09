using Netnol.Identity.Service.Application.Common;
using Netnol.Identity.Service.Application.Contracts.Inputs;
using Netnol.Identity.Service.Application.Contracts.Outputs;

namespace Netnol.Identity.Service.Application.Services;

/// <summary>
///     Defines the application-level operations for account management and authentication.
/// </summary>
public interface IAccountService
{
    /// <summary>
    ///     Checks whether an account exists for the given network identity.
    /// </summary>
    Task<Result<CheckExistenceOutput>> CheckExistenceAsync(CheckExistenceInput input);

    /// <summary>
    ///     Retrieves the public profile of an identity.
    /// </summary>
    Task<Result<ProfileOutput>> GetProfileAsync(ProfileInput input);

    /// <summary>
    ///     Creates a new account with the provided credentials.
    /// </summary>
    Task<Result<RegisterOutput>> RegisterAsync(RegisterInput input);

    /// <summary>
    ///     Generates a password authentication challenge for a given identity.
    /// </summary>
    Task<Result<PasswordChallengeOutput>> GetPasswordChallengeAsync(PasswordChallengeInput input);

    /// <summary>
    ///     Authenticates using a password-based proof.
    /// </summary>
    Task<Result<AuthenticateWithPasswordOutput>> AuthenticateWithPasswordAsync(AuthenticateWithPasswordInput input);

    /// <summary>
    ///     Authenticates using a seed-based proof.
    /// </summary>
    Task<Result<AuthenticateWithSeedOutput>> AuthenticateWithSeedAsync(AuthenticateWithSeedInput input);

    /// <summary>
    ///     Rotates the password-based credentials.
    /// </summary>
    Task<Result<RotatePasswordOutput>> RotatePasswordAsync(RotatePasswordInput input);

    /// <summary>
    ///     Rotates the seed-based credentials.
    /// </summary>
    Task<Result<RotateSeedOutput>> RotateSeedAsync(RotateSeedInput input);
}