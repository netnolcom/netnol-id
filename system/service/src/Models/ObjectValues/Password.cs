using Netnol.Identity.Core;

namespace Netnol.Identity.Service.Models.ObjectValues;

/// <summary>
///     Represents a processed security credential, containing the hash and the cryptographic parameters used for
///     derivation.
/// </summary>
/// <remarks>
///     This object follows the Zero-Knowledge Proof (ZPK) pattern, ensuring that only the
///     information necessary for verification—and not the original secret—is stored.
/// </remarks>
public readonly record struct Password
{
    public const uint HashSize = HASH.ExpectedCompute512Size;
    public const uint SaltSize = HASH.ExpectedCompute256Size;
    public const uint MinParallelism = 1;
    public const uint MaxParallelism = 10;
    public const uint MinIterations = 1;
    public const uint MaxIterations = 100;
    public const uint MinMemory = 32;
    public const uint MaxMemory = 2048;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Password" /> struct with strict parameter validation.
    /// </summary>
    /// <param name="hash">The generated hash. Must be exactly 64 bytes.</param>
    /// <param name="salt">The generated salt. Must be exactly 32 bytes.</param>
    /// <param name="iterations">Number of iterations within the allowed range.</param>
    /// <param name="memory">Memory allocation within the allowed range.</param>
    /// <param name="parallelism">Parallelism degree within the allowed range.</param>
    /// <exception cref="ArgumentException">Thrown when any parameter violates the security constraints defined in the Core.</exception>
    public Password(byte[] hash, byte[] salt, uint iterations, uint memory, uint parallelism)
    {
        if (hash.Length != HashSize)
            throw new ArgumentException($"Invalid Hash size. Expected {HashSize} bytes.");

        if (salt.Length != SaltSize)
            throw new ArgumentException($"Invalid Salt size. Expected {SaltSize} bytes.");

        if (iterations is < MinIterations or > MaxIterations)
            throw new ArgumentException(
                $"Iterations ({iterations}) must be between {MinIterations} and {MaxIterations}.");

        if (parallelism is < MinParallelism or > MaxParallelism)
            throw new ArgumentException(
                $"Parallelism ({parallelism}) must be between {MinParallelism} and {MaxParallelism}.");

        if (memory is < MinMemory or > MaxMemory)
            throw new ArgumentException($"Memory ({memory}) must be between {MinMemory} and {MaxMemory}.");

        Hash = hash;
        Salt = salt;
        Iterations = iterations;
        Memory = memory;
        Parallelism = parallelism;
    }

    /// <summary>
    ///     The resulting output of the cryptographic hash function.
    /// </summary>
    public byte[] Hash { get; }

    /// <summary>
    ///     The random salt used to protect the hash against dictionary and rainbow table attacks.
    /// </summary>
    public byte[] Salt { get; }

    /// <summary>
    ///     The number of iterations (time cost) applied to the derivation algorithm.
    /// </summary>
    public uint Iterations { get; }

    /// <summary>
    ///     The amount of memory in MiB (memory cost) allocated for the derivation process.
    /// </summary>
    public uint Memory { get; }

    /// <summary>
    ///     The degree of parallelism (number of threads) used during the derivation.
    /// </summary>
    public uint Parallelism { get; }
}