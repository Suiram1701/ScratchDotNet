using ScratchDotNet.Core.Types.Bases;
using System.Drawing;

namespace ScratchDotNet.Core.Types;

/// <summary>
/// A string value
/// </summary>
public class StringType : ScratchTypeBase
{
    /// <summary>
    /// The value this instance conatins
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Creates a new instance that contains a empty value
    /// </summary>
    public StringType() : this(string.Empty)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The value this instance should contain</param>
    public StringType(string value)
    {
        Value = value;
    }

    public override double GetNumberValue()
    {
        if (double.TryParse(Value, out double result))
            return result;
        return Value.Length;
    }

    public override string GetStringValue() =>
        Value;

    public override bool GetBoolValue()
    {
        ThrowNotSupportedException(typeof(bool));
        return false;
    }

    public override Color GetColorValue() =>
        Color.Empty;
}
