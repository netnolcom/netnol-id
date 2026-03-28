namespace Netnol.Identity.Service.Models.ObjectValues;

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
    ///     The unique 128-bit Object Identifier (OID).
    /// </summary>
    public UInt128 Value { get; }

    /// <summary>
    ///     Returns the 32-character hexadecimal representation of the OID.
    /// </summary>
    public override string ToString()
    {
        return Value.ToString("x32");
    }
}