using Microsoft.AspNetCore.Mvc;
using Netnol.Identity.Service.Application.Common;
using Netnol.Identity.Service.Application.Services;
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
        var result = await service.CheckExistenceAsync(nid);

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
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile([FromRoute] string nid)
    {
        var result = await service.GetProfileAsync(nid);

        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorDetailResponse.FromResult(result));

        return BadRequest(ErrorDetailResponse.FromResult(result));
    }

    /// <summary>
    ///     Establishes a new permanent identity with its initial cryptographic parameters.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <param name="request">The required credentials and identification data for the new account.</param>
    /// <returns>The location and details of the newly created identity.</returns>
    [HttpPost("{nid}")]
    [ProducesResponseType<AccountDetailResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromRoute] string nid, [FromBody] RegisterRequest request)
    {
        var result = await service.RegisterAsync(nid, request);

        if (result.IsSuccess)
            return StatusCode(StatusCodes.Status201Created, result.Value);

        if (result.ErrorType == ErrorType.AlreadyExists)
            return Conflict(ErrorDetailResponse.FromResult(result));

        return BadRequest(ErrorDetailResponse.FromResult(result));
    }

    /// <summary>
    ///     Commences a secure session handshake to prepare for identity verification.
    /// </summary>
    /// <param name="nid">The unique network name of the identity attempting access.</param>
    /// <returns>The cryptographic challenge and required derivation parameters.</returns>
    [HttpGet("{nid}/authentication/password/challenge")]
    [ProducesResponseType<AuthenticationChallengeWithPasswordResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AuthenticateChallengeWithPassword([FromRoute] string nid)
    {
        var result = await service.GetPasswordChallengeAsync(nid);

        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorDetailResponse.FromResult(result));

        return BadRequest(ErrorDetailResponse.FromResult(result));
    }

    /// <summary>
    ///     Validates the identity using a password-based proof of knowledge.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <param name="request">The proof of knowledge and session challenge response.</param>
    /// <returns>The authorized account material upon successful verification.</returns>
    [HttpPost("{nid}/authentication/password")]
    [ProducesResponseType<AccountDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AuthenticateWithPassword([FromRoute] string nid,
        [FromBody] PasswordAuthenticationRequest request)
    {
        var result = await service.AuthenticateWithPasswordAsync(nid, request);

        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorDetailResponse.FromResult(result));

        return BadRequest(ErrorDetailResponse.FromResult(result));
    }

    /// <summary>
    ///     Validates the identity using a master seed-based proof of knowledge.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <param name="request">The proof of knowledge and session challenge response.</param>
    /// <returns>The authorized account material upon successful verification.</returns>
    [HttpPost("{nid}/authentication/seed")]
    [ProducesResponseType<AccountDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AuthenticateWithSeed([FromRoute] string nid,
        [FromBody] SeedAuthenticationRequest request)
    {
        var result = await service.AuthenticateWithSeedAsync(nid, request);

        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorDetailResponse.FromResult(result));

        return BadRequest(ErrorDetailResponse.FromResult(result));
    }

    /// <summary>
    ///     Updates the security envelope protecting password-based credentials.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <param name="request">The updated encrypted material and verification hashes.</param>
    /// <returns>A confirmation of the credential update.</returns>
    [ProducesResponseType<AccountDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status404NotFound)]
    [HttpPut("{nid}/credentials/password")]
    public async Task<IActionResult> RotatePassword([FromRoute] string nid, [FromBody] PasswordRotationRequest request)
    {
        var result = await service.RotatePasswordAsync(nid, request);

        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorDetailResponse.FromResult(result));

        return BadRequest(ErrorDetailResponse.FromResult(result));
    }

    /// <summary>
    ///     Updates the security envelope protecting the master seed.
    /// </summary>
    /// <param name="nid">The unique network name of the identity.</param>
    /// <param name="request">The updated encrypted material and verification hashes.</param>
    /// <returns>A confirmation of the credential update.</returns>
    [HttpPut("{nid}/credentials/seed")]
    [ProducesResponseType<AccountDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorDetailResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RotateSeed([FromRoute] string nid, [FromBody] SeedRotationRequest request)
    {
        var result = await service.RotateSeedAsync(nid, request);

        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.ErrorType == ErrorType.ResourceNotFound)
            return NotFound(ErrorDetailResponse.FromResult(result));

        return BadRequest(ErrorDetailResponse.FromResult(result));
    }
}