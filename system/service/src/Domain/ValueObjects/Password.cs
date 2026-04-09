using Netnol.Identity.Core;

namespace Netnol.Identity.Service.Domain.ValueObjects;

/// <summary>
///     Represents a processed security credential, containing the hash and the cryptographic parameters used for
///     derivation.
/// </summary>
/// <remarks>
///     This object follows the Zero-Knowledge Proof (ZKP) pattern, ensuring that only the
///     information necessary for verification—and not the original secret—is stored.
/// </remarks>
public readonly record struct Password
{
    /// <summary>Expected size of the password hash (SHA‑512) in bytes.</summary>
    public const uint HashSize = HASH.ExpectedCompute512Size;

    /// <summary>Expected size of the password salt (SHA‑256) in bytes.</summary>
    public const uint SaltSize = HASH.ExpectedCompute256Size;

    /// <summary>Minimum allowed parallelism degree.</summary>
    public const uint MinParallelism = 1;

    /// <summary>Maximum allowed parallelism degree.</summary>
    public const uint MaxParallelism = 10;

    /// <summary>Minimum allowed number of iterations.</summary>
    public const uint MinIterations = 1;

    /// <summary>Maximum allowed number of iterations.</summary>
    public const uint MaxIterations = 100;

    /// <summary>Minimum allowed memory cost in MiB.</summary>
    public const uint MinMemory = 32;

    /// <summary>Maximum allowed memory cost in MiB.</summary>
    public const uint MaxMemory = 2048;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Password" /> struct with strict parameter validation.
    /// </summary>
    /// <param name="hash">The generated hash. Must be exactly <see cref="HashSize" /> bytes.</param>
    /// <param name="salt">The generated salt. Must be exactly <see cref="SaltSize" /> bytes.</param>
    /// <param name="iteration">Number of iterations within the allowed range.</param>
    /// <param name="memory">Memory allocation within the allowed range.</param>
    /// <param name="parallelism">Parallelism degree within the allowed range.</param>
    /// <exception cref="ArgumentException">Thrown when any parameter violates the security constraints.</exception>
    public Password(byte[] hash, byte[] salt, uint iteration, uint memory, uint parallelism)
    {
        if (hash.Length != HashSize)
            throw new ArgumentException($"Invalid Hash size. Expected {HashSize} bytes.");

        if (salt.Length != SaltSize)
            throw new ArgumentException($"Invalid Salt size. Expected {SaltSize} bytes.");

        if (iteration is < MinIterations or > MaxIterations)
            throw new ArgumentException(
                $"Iterations ({iteration}) must be between {MinIterations} and {MaxIterations}.");

        if (parallelism is < MinParallelism or > MaxParallelism)
            throw new ArgumentException(
                $"Parallelism ({parallelism}) must be between {MinParallelism} and {MaxParallelism}.");

        if (memory is < MinMemory or > MaxMemory)
            throw new ArgumentException($"Memory ({memory}) must be between {MinMemory} and {MaxMemory}.");

        Hash = hash;
        Salt = salt;
        Iteration = iteration;
        Memory = memory;
        Parallelism = parallelism;
    }

    /// <summary>
    ///     Gets the resulting output of the cryptographic hash function.
    /// </summary>
    public byte[] Hash { get; }

    /// <summary>
    ///     Gets the random salt used to protect the hash against dictionary and rainbow table attacks.
    /// </summary>
    public byte[] Salt { get; }

    /// <summary>
    ///     Gets the number of iterations (time cost) applied to the derivation algorithm.
    /// </summary>
    public uint Iteration { get; }

    /// <summary>
    ///     Gets the amount of memory in MiB (memory cost) allocated for the derivation process.
    /// </summary>
    public uint Memory { get; }

    /// <summary>
    ///     Gets the degree of parallelism (number of threads) used during the derivation.
    /// </summary>
    public uint Parallelism { get; }
}