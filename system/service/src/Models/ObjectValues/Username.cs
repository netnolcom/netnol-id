using Netnol.Identity.Core;

namespace Netnol.Identity.Service.Models.ObjectValues;

/// <summary>
///     Represents a Network Identity (NID) containing both its plain string value and its cryptographic hash.
/// </summary>
public readonly record struct Username
{
    public const uint HashSize = HASH.ExpectedCompute256Size;

    /// <summary>
    ///     Initializes a new instance of <see cref="Username" /> with validation and hashing.
    /// </summary>
    /// <param name="value">The raw string value for the identity.</param>
    /// <param name="hash">The pre-calculated 64-byte hash.</param>
    /// <exception cref="ArgumentException">Thrown when the value is invalid or hash size is incorrect.</exception>
    public Username(string value, byte[] hash)
    {
        if (!NID.IsValid(value))
            throw new ArgumentException($"Invalid Username format: '{value}'.");

        if (hash.Length != HashSize)
            throw new ArgumentException($"Invalid Username Hash size. Expected {HashSize} bytes.");

        Value = NID.Normalize(value);
        Hash = hash;
    }

    /// <summary>
    ///     The plain text representation of the network identity.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     The cryptographic hash of the normalized username.
    /// </summary>
    public byte[] Hash { get; }

    public override string ToString()
    {
        return Value;
    }
}