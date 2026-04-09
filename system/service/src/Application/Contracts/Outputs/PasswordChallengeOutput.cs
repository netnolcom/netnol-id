using Netnol.Identity.Service.Domain.ValueObjects;

namespace Netnol.Identity.Service.Application.Contracts.Outputs;

public record PasswordChallengeOutput(Identification Id, Username Username, byte[] PasswordSalt, uint PasswordIterationCost, uint PasswordMemoryCost, uint PasswordParallelismCost);