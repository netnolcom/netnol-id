using Microsoft.AspNetCore.Mvc;
using Netnol.Identity.Service.Application.Common;
using Netnol.Identity.Service.Application.Contracts.Inputs;
using Netnol.Identity.Service.Application.Services;
using Netnol.Identity.Service.Contracts.Common;
using Netnol.Identity.Service.Contracts.Requests;
using Netnol.Identity.Service.Contracts.Responses;

namespace Netnol.Identity.Service.Controllers;

/// <summary>
///     Provides services for identity lifecycle, state management, and secure authentication.
/// </summary>
[ApiController]
[Route("api/v1/accounts")]
[Produces("application/json")]
public class AccountController(IAccountService service) : ControllerBase
{
    /// <summary>
    ///     Verifies if a specific identity is registered within the network.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <returns>A confirmation of the identity's existence.</returns>
    [HttpHead("{nid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckExistence([FromRoute] string nid)
    {
        var result = await service.CheckExistenceAsync(new CheckExistenceInput(nid));

        if (result.IsSuccess)
            return StatusCode(StatusCodes.Status200OK);

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return StatusCode(StatusCodes.Status404NotFound);

        return BadRequest();
    }

    /// <summary>
    ///     Obtains the public credentials required to interact with a specific identity.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <returns>The public cryptographic profile and unique identifier.</returns>
    [HttpGet("{nid}")]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Profile([FromRoute] string nid)
    {
        var result = await service.GetProfileAsync(new ProfileInput(nid));

        if (result.IsSuccess)
            return Ok(ProfileResponse.FromOutput(result.Value));

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorMessageResponse.FromResult(result));

        return BadRequest(ErrorMessageResponse.FromResult(result));
    }

    /// <summary>
    ///     Establishes a new permanent identity with its initial cryptographic parameters.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <param name="request">The required credentials and identification data for the new account.</param>
    /// <returns>The location and details of the newly created identity.</returns>
    [HttpPost("{nid}")]
    [ProducesResponseType<RegisterResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromRoute] string nid, [FromBody] RegisterRequest request)
    {
        var result = await service.RegisterAsync(new RegisterInput(
            Username: nid,
            PublicKey: request.PublicKey,
            PublicKeyHash: request.PublicKeyHash,
            PrivateKeyHash: request.PrivateKeyHash,
            EncryptedPrivateKeyWithPassword: request.EncryptedPrivateKeyWithPassword,
            EncryptedPrivateKeyWithSeed: request.EncryptedPrivateKeyWithSeed,
            EncryptedSeedWithMasterKey: request.EncryptedSeedWithPrivateKey,
            SeedHash: request.SeedHash,
            PasswordHash: request.PasswordHash,
            PasswordSalt: request.PasswordSalt,
            PasswordMemoryCost: request.PasswordMemoryCost,
            PasswordParallelismCost: request.PasswordParallelismCost,
            PasswordIterationCost: request.PasswordIterationCost));

        if (result.IsSuccess)
            return StatusCode(StatusCodes.Status201Created, RegisterResponse.FromOutput(result.Value));

        if (result.ErrorType == ErrorType.AlreadyExists)
            return Conflict(ErrorMessageResponse.FromResult(result));

        return BadRequest(ErrorMessageResponse.FromResult(result));
    }

    /// <summary>
    ///     Commences a secure session handshake to prepare for identity verification.
    /// </summary>
    /// <param name="nid">The unique network name of the identity attempting access.</param>
    /// <returns>The cryptographic challenge and required derivation parameters.</returns>
    [HttpGet("{nid}/authentication/password/challenge")]
    [ProducesResponseType<PasswordChallengeResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PasswordChallenge([FromRoute] string nid)
    {
        var result = await service.GetPasswordChallengeAsync(new PasswordChallengeInput(nid));

        if (result.IsSuccess)
            return Ok(PasswordChallengeResponse.FromOutput(result.Value));

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorMessageResponse.FromResult(result));

        return BadRequest(ErrorMessageResponse.FromResult(result));
    }

    /// <summary>
    ///     Validates the identity using a password-based proof of knowledge.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <param name="request">The proof of knowledge and session challenge response.</param>
    /// <returns>The authorized account material upon successful verification.</returns>
    [HttpPost("{nid}/authentication/password")]
    [ProducesResponseType<AuthenticateWithPasswordResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AuthenticateWithPassword([FromRoute] string nid,
        [FromBody] AuthenticateWithPasswordRequest request)
    {
        var result = await service.AuthenticateWithPasswordAsync(new AuthenticateWithPasswordInput(nid, request.PasswordHash));

        if (result.IsSuccess)
            return Ok(AuthenticateWithPasswordResponse.FromOutput(result.Value));

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorMessageResponse.FromResult(result));

        return BadRequest(ErrorMessageResponse.FromResult(result));
    }

    /// <summary>
    ///     Validates the identity using a master seed-based proof of knowledge.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <param name="request">The proof of knowledge and session challenge response.</param>
    /// <returns>The authorized account material upon successful verification.</returns>
    [HttpPost("{nid}/authentication/seed")]
    [ProducesResponseType<AuthenticateWithSeedResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AuthenticateWithSeed([FromRoute] string nid,
        [FromBody] AuthenticateWithSeedRequest request)
    {
        var result = await service.AuthenticateWithSeedAsync(new AuthenticateWithSeedInput(nid, request.SeedHash));

        if (result.IsSuccess)
            return Ok(AuthenticateWithSeedResponse.FromOutput(result.Value));

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorMessageResponse.FromResult(result));

        return BadRequest(ErrorMessageResponse.FromResult(result));
    }

    /// <summary>
    ///     Updates the security envelope protecting password-based credentials.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <param name="request">The updated encrypted material and verification hashes.</param>
    /// <returns>A confirmation of the credential update.</returns>
    [ProducesResponseType<RotatePasswordResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status404NotFound)]
    [HttpPut("{nid}/credentials/password")]
    public async Task<IActionResult> RotatePassword([FromRoute] string nid, [FromBody] RotatePasswordRequest request)
    {
        var result =
            await service.RotatePasswordAsync(new RotatePasswordInput(
                Username: nid,
                OldPasswordHash: request.OldPasswordHash,
                NewPasswordHash: request.NewPasswordHash,
                NewPasswordSalt: request.NewPasswordSalt,
                EncryptedPrivateKeyWithNewPassword: request.EncryptedPrivateKeyWithNewPassword,
                NewPasswordMemoryCost: request.NewPasswordMemoryCost,
                NewPasswordParallelizationCost: request.NewPasswordParallelismCost,
                NewPasswordIterationCost: request.NewPasswordIterationCost));

        if (result.IsSuccess)
            return Ok(RotatePasswordResponse.FromOutput(result.Value));

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorMessageResponse.FromResult(result));

        return BadRequest(ErrorMessageResponse.FromResult(result));
    }

    /// <summary>
    ///     Updates the security envelope protecting the master seed.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <param name="request">The updated encrypted material and verification hashes.</param>
    /// <returns>A confirmation of the credential update.</returns>
    [HttpPut("{nid}/credentials/seed")]
    [ProducesResponseType<RotateSeedResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorMessageResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RotateSeed([FromRoute] string nid, [FromBody] RotateSeedRequest request)
    {
        var result = await service.RotateSeedAsync(new RotateSeedInput(Username:nid,
            OldSeedHash: request.OldSeedHash,
            NewSeedHash: request.NewSeedHash,
            EncryptedSeedWithMasterKey: request.EncryptedSeedWithMasterKey,
            EncryptedPrivateKeyWithNewSeed: request.EncryptedPrivateKeyWithNewSeed));

        if (result.IsSuccess)
            return Ok(RotateSeedResponse.FromOutput(result.Value));

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorMessageResponse.FromResult(result));

        return BadRequest(ErrorMessageResponse.FromResult(result));
    }
}