using Scratch.Core.Types.Bases;
using System.Drawing;

namespace Scratch.Core.Types;

/// <summary>
/// A number value
/// </summary>
public class NumberType : ScratchTypeBase
{
    /// <summary>
    /// The value contained by this instance
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Creates a new instance that conatins an empty value
    /// </summary>
    public NumberType() : this(0d)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The value this instance should contains</param>
    public NumberType(double value)
    {
        Value = value;
    }

    public override double GetNumberValue() =>
        Value;

    public override string GetStringValue() =>
        Value.ToString();

    public override bool GetBoolValue()
    {
        ThrowNotSupportedException(typeof(bool));
        return false;
    }

    public override Color GetColorValue() =>
        Color.Empty;
}
