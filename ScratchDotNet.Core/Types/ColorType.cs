using ScratchDotNet.Core.Types.Bases;
using System.Drawing;

namespace ScratchDotNet.Core.Types;

/// <summary>
/// A color
/// </summary>
public class ColorType : ScratchTypeBase
{
    /// <summary>
    /// The value this instance contains
    /// </summary>
    public Color Value { get; set; }

    /// <summary>
    /// Creates a new instance that contains an empty value
    /// </summary>
    public ColorType() : this(Color.Empty)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The value this instance should contain</param>
    public ColorType(Color value)
    {
        Value = value;
    }

    public override double GetNumberValue()
    {
        ThrowNotSupportedException(typeof(double));
        return 0d;
    }

    public override string GetStringValue() =>
        Value.ToString();

    public override bool GetBoolValue()
    {
        ThrowNotSupportedException(typeof(bool));
        return false;
    }

    public override Color GetColorValue() =>
        Value;
}
