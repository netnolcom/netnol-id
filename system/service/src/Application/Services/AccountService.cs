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
            return Result<CheckExistenceOutput>.FromFailure(ErrorType.InvalidInput, "Invalid username format.");

        try
        {
            var account = await repository.GetByUsernameAsync(username);

            return account == null
                ? Result<CheckExistenceOutput>.FromFailure(ErrorType.ResourceNotFound, "Account not found.")
                : Result<CheckExistenceOutput>.FromSuccess(new CheckExistenceOutput(account.Id, account.Username),
                    "Account found successfully.");
        }
        catch (Exception e)
        {
            return Result<CheckExistenceOutput>.FromFailure(ErrorType.Unexpected, e.Message);
        }
    }

    /// <inheritdoc /> 
    public async Task<Result<ProfileOutput>> GetProfileAsync(ProfileInput input)
    {
        if (!Username.TryParse(input.Username, out var username))
            return Result<ProfileOutput>.FromFailure(ErrorType.InvalidInput, "Invalid username format.");

        try
        {
            var account = await repository.GetByUsernameAsync(username);

            return account == null
                ? Result<ProfileOutput>.FromFailure(ErrorType.ResourceNotFound, "Account not found.")
                : Result<ProfileOutput>.FromSuccess(
                    new ProfileOutput(account.Id, account.Username, account.Keys.Public),
                    "Account profile found successfully.");
        }
        catch (Exception e)
        {
            return Result<ProfileOutput>.FromFailure(ErrorType.Unexpected, e.Message);
        }
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

        try
        {
            var id = new Identification(OID.New());

            var keys = new KeyPair(
                publicKey: CONVERTER.ToBinary(input.PublicKey),
                publicHash: CONVERTER.ToBinary(input.PublicKeyHash),
                privateHash: CONVERTER.ToBinary(input.PrivateKeyHash),
                privateWithPassword: CONVERTER.ToBinary(input.EncryptedPrivateKeyWithPassword),
                privateWithSeed: CONVERTER.ToBinary(input.EncryptedPrivateKeyWithSeed)
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

            var account = new Account(id, username, keys, seed, password);

            await repository.AddAsync(account);

            return Result<RegisterOutput>.FromSuccess(new RegisterOutput(account.Id, account.Username,
                account.Keys.Public), "Account registered successfully.");
        }
        catch (Exception e)
        {
            return Result<RegisterOutput>.FromFailure(ErrorType.Unexpected, e.Message);
        }
    }

    /// <inheritdoc /> 
    public async Task<Result<PasswordChallengeOutput>> GetPasswordChallengeAsync(PasswordChallengeInput input)
    {
        if (!Username.TryParse(input.Username, out var username))
            return Result<PasswordChallengeOutput>.FromFailure(ErrorType.InvalidInput, "Invalid username format.");

        try
        {
            var account = await repository.GetByUsernameAsync(username);

            if (account == null)
                return Result<PasswordChallengeOutput>.FromFailure(ErrorType.ResourceNotFound, "Account not found.");

            return Result<PasswordChallengeOutput>.FromSuccess(new PasswordChallengeOutput(
                    Id: account.Id,
                    Username: account.Username,
                    PasswordSalt: account.Password.Salt,
                    PasswordIterationCost: account.Password.Iteration,
                    PasswordMemoryCost: account.Password.Memory,
                    PasswordParallelismCost: account.Password.Parallelism),
                "Password challenge found successfully.");
        }
        catch (Exception e)
        {
            return Result<PasswordChallengeOutput>.FromFailure(ErrorType.Unexpected, e.Message);
        }
    }

    /// <inheritdoc /> 
    public async Task<Result<AuthenticateWithPasswordOutput>> AuthenticateWithPasswordAsync(
        AuthenticateWithPasswordInput input)
    {
        if (!Username.TryParse(input.Username, out var username))
            return Result<AuthenticateWithPasswordOutput>.FromFailure(ErrorType.InvalidInput,
                "Invalid username format.");

        if (string.IsNullOrWhiteSpace(input.PasswordHash))
            return Result<AuthenticateWithPasswordOutput>.FromFailure(ErrorType.InvalidInput, "Invalid password hash.");

        try
        {
            var passwordHash = CONVERTER.ToBinary(input.PasswordHash);

            if (passwordHash.Length != Password.HashSize)
                return Result<AuthenticateWithPasswordOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid password hash length.");

            var account = await repository.GetByUsernameAsync(username);

            if (account == null)
                return Result<AuthenticateWithPasswordOutput>.FromFailure(ErrorType.ResourceNotFound,
                    "Account not found.");

            if (!HASH.ConstantTimeAreEqual(account.Password.Hash, passwordHash))
                return Result<AuthenticateWithPasswordOutput>.FromFailure(ErrorType.PermissionDenied,
                    "Invalid password authentication.");

            return Result<AuthenticateWithPasswordOutput>.FromSuccess(new AuthenticateWithPasswordOutput(
                    Id:
                    account.Id,
                    Username: account.Username,
                    PublicKey: account.Keys.Public,
                    PublicKeyHash: account.Keys.PublicHash,
                    EncryptedPrivateKeyWithPassword: account.Keys.PrivateWithPassword,
                    PrivateKeyHash: account.Keys.PrivateHash),
                "Authenticate with password successfully.");
        }
        catch (Exception e)
        {
            return Result<AuthenticateWithPasswordOutput>.FromFailure(ErrorType.Unexpected, e.Message);
        }
    }

    /// <inheritdoc /> 
    public async Task<Result<AuthenticateWithSeedOutput>> AuthenticateWithSeedAsync(AuthenticateWithSeedInput input)
    {
        if (!Username.TryParse(input.Username, out var username))
            return Result<AuthenticateWithSeedOutput>.FromFailure(ErrorType.InvalidInput,
                "Invalid username format.");

        if (string.IsNullOrWhiteSpace(input.SeedHash))
            return Result<AuthenticateWithSeedOutput>.FromFailure(ErrorType.InvalidInput, "Invalid seed hash.");

        try
        {
            var seedHash = CONVERTER.ToBinary(input.SeedHash);

            if (seedHash.Length != Seed.HashSize)
                return Result<AuthenticateWithSeedOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid seed hash length.");

            var account = await repository.GetByUsernameAsync(username);

            if (account == null)
                return Result<AuthenticateWithSeedOutput>.FromFailure(ErrorType.ResourceNotFound,
                    "Account not found.");

            if (!HASH.ConstantTimeAreEqual(account.Seed.Hash, seedHash))
                return Result<AuthenticateWithSeedOutput>.FromFailure(ErrorType.PermissionDenied,
                    "Invalid seed authentication.");

            return Result<AuthenticateWithSeedOutput>.FromSuccess(new AuthenticateWithSeedOutput(
                    Id: account.Id,
                    Username: account.Username,
                    PublicKey: account.Keys.Public,
                    PublicKeyHash: account.Keys.PublicHash,
                    EncryptedPrivateKeyWithSeed: account.Keys.PrivateWithSeed,
                    PrivateKeyHash: account.Keys.PrivateHash,
                    EncryptedSeedWithMasterKey: account.Seed.EncryptedValue),
                "Authenticate with password successfully.");
        }
        catch (Exception e)
        {
            return Result<AuthenticateWithSeedOutput>.FromFailure(ErrorType.Unexpected, e.Message);
        }
    }

    /// <inheritdoc /> 
    public async Task<Result<RotatePasswordOutput>> RotatePasswordAsync(RotatePasswordInput input)
    {
        if (!Username.TryParse(input.Username, out var username))
            return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput, "Invalid username format.");

        if (string.IsNullOrWhiteSpace(input.OldPasswordHash))
            return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput, "Invalid old password hash.");

        if (string.IsNullOrWhiteSpace(input.NewPasswordHash))
            return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput, "Invalid new password hash.");

        if (string.IsNullOrWhiteSpace(input.EncryptedPrivateKeyWithNewPassword))
            return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput,
                "Invalid encrypted private key with new password.");

        if (string.IsNullOrWhiteSpace(input.NewPasswordSalt))
            return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput, "Invalid new password salt.");

        try
        {
            var oldPasswordHash = CONVERTER.ToBinary(input.OldPasswordHash);
            var newPasswordHash = CONVERTER.ToBinary(input.NewPasswordHash);
            var encryptedPrivateKeyWithNewPassword = CONVERTER.ToBinary(input.EncryptedPrivateKeyWithNewPassword);
            var passwordParallelismCost = input.NewPasswordParallelizationCost;
            var passwordMemoryCost = input.NewPasswordMemoryCost;
            var passwordIterationCost = input.NewPasswordIterationCost;
            var newPasswordSalt = CONVERTER.ToBinary(input.NewPasswordSalt);

            if (oldPasswordHash.Length != Seed.HashSize)
                return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid old password hash length.");

            if (newPasswordHash.Length != Seed.HashSize)
                return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid new password hash length.");

            if (encryptedPrivateKeyWithNewPassword.Length != KeyPair.EncryptedPrivateKeySize)
                return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid encrypted private key with new password length.");

            if (passwordIterationCost is < Password.MinIterations or > Password.MaxIterations)
                return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid password iteration cost value.");

            if (passwordMemoryCost is < Password.MinMemory or > Password.MaxMemory)
                return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid password memory cost value.");

            if (passwordParallelismCost is < Password.MinParallelism or > Password.MaxParallelism)
                return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid password parallelism cost value.");

            if (newPasswordSalt.Length != Password.SaltSize)
                return Result<RotatePasswordOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid new password salt length.");

            var account = await repository.GetByUsernameAsync(username);

            if (account == null)
                return Result<RotatePasswordOutput>.FromFailure(ErrorType.ResourceNotFound, "Account not found.");


            if (!HASH.ConstantTimeAreEqual(account.Password.Hash, oldPasswordHash))
                return Result<RotatePasswordOutput>.FromFailure(ErrorType.PermissionDenied,
                    "Invalid password authentication.");

            account.UpdatePassword(
                encryptedPrivateKeyWithNewPassword: encryptedPrivateKeyWithNewPassword,
                newPasswordHash: newPasswordHash,
                passwordParallelismCost: passwordParallelismCost,
                passwordMemoryCost: passwordMemoryCost,
                passwordIterationCost: passwordIterationCost,
                newPasswordSalt: newPasswordSalt);

            await repository.UpdateAsync(account);

            return Result<RotatePasswordOutput>.FromSuccess(new RotatePasswordOutput(account.Id, account.Username,
                account.Keys.Public), "Seed rotated successfully.");
        }
        catch (Exception e)
        {
            return Result<RotatePasswordOutput>.FromFailure(ErrorType.Unexpected, e.Message);
        }
    }

    /// <inheritdoc /> 
    public async Task<Result<RotateSeedOutput>> RotateSeedAsync(RotateSeedInput input)
    {
        if (!Username.TryParse(input.Username, out var username))
            return Result<RotateSeedOutput>.FromFailure(ErrorType.InvalidInput, "Invalid username format.");

        if (string.IsNullOrWhiteSpace(input.OldSeedHash))
            return Result<RotateSeedOutput>.FromFailure(ErrorType.InvalidInput, "Invalid old seed hash.");

        if (string.IsNullOrWhiteSpace(input.NewSeedHash))
            return Result<RotateSeedOutput>.FromFailure(ErrorType.InvalidInput, "Invalid new seed hash.");

        if (string.IsNullOrWhiteSpace(input.EncryptedSeedWithMasterKey))
            return Result<RotateSeedOutput>.FromFailure(ErrorType.InvalidInput,
                "Invalid encrypted seed with master key.");

        if (string.IsNullOrWhiteSpace(input.EncryptedPrivateKeyWithNewSeed))
            return Result<RotateSeedOutput>.FromFailure(ErrorType.InvalidInput,
                "Invalid encrypted private key with new seed.");

        try
        {
            var oldSeedHash = CONVERTER.ToBinary(input.OldSeedHash);
            var newSeedHash = CONVERTER.ToBinary(input.NewSeedHash);
            var encryptedSeedWithMasterKey = CONVERTER.ToBinary(input.EncryptedSeedWithMasterKey);
            var encryptedPrivateKeyWithNewSeed = CONVERTER.ToBinary(input.EncryptedPrivateKeyWithNewSeed);

            if (oldSeedHash.Length != Seed.HashSize)
                return Result<RotateSeedOutput>.FromFailure(ErrorType.InvalidInput, "Invalid old seed hash length.");

            if (newSeedHash.Length != Seed.HashSize)
                return Result<RotateSeedOutput>.FromFailure(ErrorType.InvalidInput, "Invalid new seed hash length.");

            if (encryptedSeedWithMasterKey.Length != Seed.EncryptedSeedSize)
                return Result<RotateSeedOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid encrypted seed with master key length.");

            if (encryptedPrivateKeyWithNewSeed.Length != KeyPair.EncryptedPrivateKeySize)
                return Result<RotateSeedOutput>.FromFailure(ErrorType.InvalidInput,
                    "Invalid encrypted private key with new seed length.");

            var account = await repository.GetByUsernameAsync(username);

            if (account == null)
                return Result<RotateSeedOutput>.FromFailure(ErrorType.ResourceNotFound, "Account not found.");

            if (!HASH.ConstantTimeAreEqual(account.Seed.Hash, oldSeedHash))
                return Result<RotateSeedOutput>.FromFailure(ErrorType.PermissionDenied, "Invalid seed authentication.");

            account.UpdateSeed(
                encryptedSeedWithMasterKey: encryptedSeedWithMasterKey,
                encryptedPrivateKeyWithNewSeed: encryptedPrivateKeyWithNewSeed,
                newSeedHash: newSeedHash);

            await repository.UpdateAsync(account);

            return Result<RotateSeedOutput>.FromSuccess(new RotateSeedOutput(account.Id, account.Username,
                account.Keys.Public), "Seed rotated successfully.");
        }
        catch (Exception e)
        {
            return Result<RotateSeedOutput>.FromFailure(ErrorType.Unexpected, e.Message);
        }
    }
}