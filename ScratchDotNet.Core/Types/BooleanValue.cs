using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Types;

/// <summary>
/// A boolean value that is dynamically convertible and comparable with other scratch types
/// </summary>
/// <param name="value">The initialized value</param>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct BooleanValue(bool value) : IScratchType
{
    /// <inheritdoc/>
    /// <remarks>
    /// This will be never get called
    /// </remarks>
    public event EventHandler<ValueChangedEventArgs> OnValueChanged { add { } remove { } }

    /// <summary>
    /// The value this instance contains
    /// </summary>
    public readonly bool Value = value;

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        Equals(obj as IScratchType);

    public override int GetHashCode() =>
        Value.GetHashCode();

    public DoubleValue ConvertToDoubleValue()
    {
        double result = Convert.ToDouble(Value);
        return new(result);
    }

    public StringValue ConvertToStringValue() =>
        new(Value.ToString());

    public bool Equals(IScratchType? other)
    {
        if (other is null)
            return false;

        if (other is BooleanValue otherBool)
            return Value == otherBool.Value;

        if (other is DoubleValue)
        {
            DoubleValue boolDouble = ConvertToDoubleValue();
            return boolDouble.Equals(other);
        }

        if (other is StringValue otherString)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;     // this comparer do exactly the comparison as scratch
            return comparer.Equals(Value.ToString(), otherString.Value);
        }

        return false;     // By default is a boolean value not equal another value
    }

    public int CompareTo(IScratchType? other)
    {
        if (other is BooleanValue otherBool)
            return Value.CompareTo(otherBool.Value);

        if (other is DoubleValue)
        {
            DoubleValue boolDouble = ConvertToDoubleValue();
            return boolDouble.CompareTo(other);
        }

        if (other is StringValue otherString)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;     // this comparer do exactly the comparison as scratch

            if (comparer.Equals(Value.ToString(), otherString.Value))
                return 0;
            else if (!Value && comparer.Equals(bool.TrueString, otherString.Value))     // When this is False and other is True return -1
                return -1;
        }

        return 1;     // By default is a boolean value larger than aonther value
    }

    public static bool operator ==(BooleanValue left, BooleanValue right) =>
        left.Equals(right);

    public static bool operator !=(BooleanValue left, BooleanValue right) =>
        !left.Equals(right);

    public static bool operator <(BooleanValue left, BooleanValue right) =>
        left.CompareTo(right) < 0;

    public static bool operator <=(BooleanValue left, BooleanValue right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >(BooleanValue left, BooleanValue right) =>
        left.CompareTo(right) > 0;

    public static bool operator >=(BooleanValue left, BooleanValue right) =>
        left.CompareTo(right) >= 0;

    public static implicit operator bool(BooleanValue value) =>
        value.Value;

    private string GetDebuggerDisplay() =>
        Value.ToString();
}
