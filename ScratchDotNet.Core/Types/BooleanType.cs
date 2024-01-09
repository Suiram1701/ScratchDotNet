using ScratchDotNet.Core.Types.Bases;
using System.Drawing;

namespace ScratchDotNet.Core.Types;

/// <summary>
/// A boolean value
/// </summary>
public class BooleanType : ScratchTypeBase
{
    /// <summary>
    /// The value this instance contains
    /// </summary>
    public bool Value { get; set; }

    /// <summary>
    /// Creates a new instance that contains an empty value
    /// </summary>
    public BooleanType() : this(false)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The value this instance should contain</param>
    public BooleanType(bool value)
    {
        Value = value;
    }

    public override double GetNumberValue() =>
        Value
            ? 1
            : 0;

    public override string GetStringValue() =>
        Value.ToString();

    public override bool GetBoolValue() =>
        Value;

    public override Color GetColorValue()
    {
        ThrowNotSupportedException(typeof(Color));
        return Color.Empty;
    }
}
