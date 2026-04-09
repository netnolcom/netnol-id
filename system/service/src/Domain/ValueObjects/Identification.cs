using System.Globalization;

namespace Netnol.Identity.Service.Domain.ValueObjects;

/// <summary>
///     Represents the unique machine-level identity using a 128-bit Object Identifier (OID).
/// </summary>
/// <remarks>
///     As a numeric value, it provides native high-performance indexing.
/// </remarks>
public readonly record struct Identification
{
    /// <summary>
    ///     Initializes a new instance of <see cref="Identification" /> with the provided OID.
    /// </summary>
    /// <param name="value">The 128-bit OID value. Cannot be zero.</param>
    /// <exception cref="ArgumentException">Thrown when the OID is empty (Zero).</exception>
    public Identification(UInt128 value)
    {
        if (value <= UInt128.Zero)
            throw new ArgumentException("Identification value (OID) cannot be zero.");

        Value = value;
    }

    /// <summary>
    ///     Gets the unique 128-bit Object Identifier (OID).
    /// </summary>
    public UInt128 Value { get; }

    /// <summary>
    ///     Returns the 32-character hexadecimal representation of the OID.
    /// </summary>
    /// <returns>A 32-character lowercase hexadecimal string.</returns>
    public override string ToString()
    {
        return Value.ToString("x32");
    }

    public static Identification Parse(string value)
    {
        return new Identification(UInt128.Parse(value, NumberStyles.HexNumber));
    }
}