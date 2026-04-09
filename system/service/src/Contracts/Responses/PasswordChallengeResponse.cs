using Netnol.Identity.Service.Application.Contracts.Outputs;

namespace Netnol.Identity.Service.Contracts.Responses;

/// <summary>
///     Provides the parameters required for a client-side password derivation challenge.
/// </summary>
/// <param name="PasswordSalt">The unique account salt.</param>
/// <param name="PasswordIterationCost">The required computational passes.</param>
/// <param name="PasswordMemoryCost">The required memory allocation in MiB.</param>
/// <param name="PasswordParallelismCost">The required thread concurrency.</param>
public record PasswordChallengeResponse(
    string Id,
    string Username,
    string PasswordSalt,
    uint PasswordIterationCost,
    uint PasswordMemoryCost,
    uint PasswordParallelismCost)
{
    public static PasswordChallengeResponse FromOutput(PasswordChallengeOutput output)
    {
        return new PasswordChallengeResponse(
            Id: output.Id.ToString(),
            Username: output.Username.Value,
            PasswordSalt: output.PasswordSalt,
            PasswordIterationCost: output.PasswordIterationCost,
            PasswordMemoryCost: output.PasswordMemoryCost,
            PasswordParallelismCost: output.PasswordParallelismCost);
    }
}