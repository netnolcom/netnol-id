using Netnol.Identity.Core;
using Netnol.Identity.Service.Application.Common;
using Netnol.Identity.Service.Contracts.Requests;
using Netnol.Identity.Service.Contracts.Responses;
using Netnol.Identity.Service.Domain.Repositories;
using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Application.Services;

/// <inheritdoc />
public class AccountService(IAccountRepository repository) : IAccountService
{
    /// <inheritdoc />
    public async Task<Result<bool>> CheckExistenceAsync(string nid)
    {
        if (!Username.TryParse(nid, out var username))
            return Result<bool>.FromFailure(ErrorType.InvalidInput);

        var account = await repository.GetByUsernameAsync(username);

        return account != null
            ? Result<bool>.FromSuccess(true)
            : Result<bool>.FromFailure(ErrorType.ResourceNotFound);
    }

    /// <inheritdoc />
    public async Task<Result<ProfileResponse>> GetProfileAsync(string nid)
    {
        if (!Username.TryParse(nid, out var username))
            return Result<ProfileResponse>.FromFailure(ErrorType.InvalidInput);

        var account = await repository.GetByUsernameAsync(username);

        return account != null
            ? Result<ProfileResponse>.FromSuccess(new ProfileResponse(account.Id.Value, account.Username.Value,
                CONVERTER.FromBinary(account.Keys.Public)))
            : Result<ProfileResponse>.FromFailure(ErrorType.ResourceNotFound);
    }

    /// <inheritdoc />
    public async Task<Result<AccountDetailResponse>> RegisterAsync(string nid, RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<Result<AuthenticationChallengeWithPasswordResponse>> GetPasswordChallengeAsync(string nid)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<Result<AccountDetailResponse>> AuthenticateWithPasswordAsync(string nid,
        PasswordAuthenticationRequest request)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<Result<AccountDetailResponse>> AuthenticateWithSeedAsync(string nid,
        SeedAuthenticationRequest request)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<Result<AccountDetailResponse>> RotatePasswordAsync(string nid, PasswordRotationRequest request)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<Result<AccountDetailResponse>> RotateSeedAsync(string nid, SeedRotationRequest request)
    {
        throw new NotImplementedException();
    }
}