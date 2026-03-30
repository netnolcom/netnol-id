namespace Netnol.Identity.Service.Contracts.Responses;

/// <summary>
///     Provides the parameters required for a client-side password derivation challenge.
/// </summary>
/// <param name="Salt">The unique account salt.</param>
/// <param name="Iterations">The required computational passes.</param>
/// <param name="Memory">The required memory allocation in MiB.</param>
/// <param name="Parallelism">The required thread concurrency.</param>
public record AuthenticationChallengeWithPasswordResponse(
    string Salt,
    uint Iterations,
    uint Memory,
    uint Parallelism);