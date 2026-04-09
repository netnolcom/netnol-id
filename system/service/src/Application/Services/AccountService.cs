using Netnol.Identity.Core;
using Netnol.Identity.Service.Application.Common;
using Netnol.Identity.Service.Application.Contracts.Inputs;
using Netnol.Identity.Service.Application.Contracts.Outputs;
using Netnol.Identity.Service.Domain.Entities;
using Netnol.Identity.Service.Domain.Repositories;
using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Application.Services;

/// <inheritdoc />
public class AccountService(IAccountRepository repository) : IAccountService
{
    /// <inheritdoc />
    public async Task<Result<CheckExistenceOutput>> CheckExistenceAsync(CheckExistenceInput input)
    {
        if (!Username.TryParse(input.Username, out var username))
            return Result<CheckExistenceOutput>.FromFailure(ErrorType.InvalidInput);

        var account = await repository.GetByUsernameAsync(username);

        return account == null
            ? Result<CheckExistenceOutput>.FromFailure(ErrorType.ResourceNotFound)
            : Result<CheckExistenceOutput>.FromSuccess(new CheckExistenceOutput(account.Id, account.Username));
    }

    /// <inheritdoc /> 
    public async Task<Result<ProfileOutput>> GetProfileAsync(ProfileInput input)
    {
        if (!Username.TryParse(input.Username, out var username))
            return Result<ProfileOutput>.FromFailure(ErrorType.InvalidInput);

        var account = await repository.GetByUsernameAsync(username);

        return account == null
            ? Result<ProfileOutput>.FromFailure(ErrorType.ResourceNotFound)
            : Result<ProfileOutput>.FromSuccess(new ProfileOutput(account.Id, account.Username, account.Keys.Public));
    }

    /// <inheritdoc /> 
    public async Task<Result<RegisterOutput>> RegisterAsync(RegisterInput input)
    {
        if (!Username.TryParse(input.Username, out var username))
            return Result<RegisterOutput>.FromFailure(ErrorType.InvalidInput, "Invalid username format.");

        var foundAccount = await repository.GetByUsernameAsync(username);
        if (foundAccount != null)
            return Result<RegisterOutput>.FromFailure(ErrorType.AlreadyExists, "Username already exists.");

        if (string.IsNullOrWhiteSpace(input.PublicKey))
            return Result<RegisterOutput>.FromFailure(ErrorType.InvalidInput, "Invalid keypair public key.");

        if (string.IsNullOrWhiteSpace(input.PublicKeyHash))
            return Result<RegisterOutput>.FromFailure(ErrorType.InvalidInput, "Invalid keypair public key hash.");

        if (string.IsNullOrWhiteSpace(input.PrivateKeyHash))
            return Result<RegisterOutput>.FromFailure(ErrorType.InvalidInput, "Invalid keypair private key hash.");

        if (string.IsNullOrWhiteSpace(input.SeedHash))
            return Result<RegisterOutput>.FromFailure(ErrorType.InvalidInput, "Invalid seed hash.");

        if (string.IsNullOrWhiteSpace(input.PasswordHash))
            return Result<RegisterOutput>.FromFailure(ErrorType.InvalidInput, "Invalid password hash.");

        if (string.IsNullOrWhiteSpace(input.EncryptedSeedWithMasterKey))
            return Result<RegisterOutput>.FromFailure(ErrorType.InvalidInput,
                "Invalid encrypted seed with master key.");

        if (string.IsNullOrWhiteSpace(input.EncryptedPrivateKeyWithPassword))
            return Result<RegisterOutput>.FromFailure(ErrorType.InvalidInput,
                "Invalid encrypted private key with password.");

        if (string.IsNullOrWhiteSpace(input.EncryptedPrivateKeyWithSeed))
            return Result<RegisterOutput>.FromFailure(ErrorType.InvalidInput,
                "Invalid encrypted private key with seed.");

        Account account;

        try
        {
            var id = new Identification(OID.New());

            var keys = new KeyPair(
                publicKey: CONVERTER.ToBinary(input.PublicKey),
                publicHash: CONVERTER.ToBinary(input.PublicKeyHash),
                privateHash: CONVERTER.ToBinary(input.PrivateKeyHash),
                privateWithPassword: CONVERTER.ToBinary(input.EncryptedPrivateKeyWithPassword),
                privateWithSalt: CONVERTER.ToBinary(input.EncryptedPrivateKeyWithSeed)
            );

            var seed = new Seed(
                encryptedValue: CONVERTER.ToBinary(input.EncryptedSeedWithMasterKey),
                hash: CONVERTER.ToBinary(input.SeedHash)
            );

            var password = new Password(
                hash: CONVERTER.ToBinary(input.PasswordHash),
                salt: CONVERTER.ToBinary(input.PasswordSalt),
                iteration: input.PasswordIterationCost,
                memory: input.PasswordMemoryCost,
                parallelism: input.PasswordParallelismCost
            );

            account = new Account(id, username, keys, seed, password);
        }
        catch (Exception e)
        {
            return Result<RegisterOutput>.FromFailure(ErrorType.InvalidInput, e.Message);
        }

        await repository.AddAsync(account);

        return Result<RegisterOutput>.FromSuccess(new RegisterOutput(account.Id, account.Username, account.Keys.Public));
    }

    /// <inheritdoc /> 
    public async Task<Result<PasswordChallengeOutput>> GetPasswordChallengeAsync(PasswordChallengeInput input)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc /> 
    public async Task<Result<AuthenticateWithPasswordOutput>> AuthenticateWithPasswordAsync(
        AuthenticateWithPasswordInput input)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc /> 
    public async Task<Result<AuthenticateWithSeedOutput>> AuthenticateWithSeedAsync(AuthenticateWithSeedInput input)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc /> 
    public async Task<Result<RotatePasswordOutput>> RotatePasswordAsync(RotatePasswordInput input)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc /> 
    public async Task<Result<RotateSeedOutput>> RotateSeedAsync(RotateSeedInput input)
    {
        throw new NotImplementedException();
    }
}