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
/// A string value that is dynamically convertible and comparable with other scratch types
/// </summary>
/// <param name="value">The initialized value</param>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class StringValue(string value) : IScratchType
{
    /// <summary>
    /// The string value this instance contains
    /// </summary>
    public string Value { get; } = value;

    /// <summary>
    /// Creates an empty instance
    /// </summary>
    public StringValue() : this(string.Empty)
    {
    }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        Equals(obj as IScratchType);

    public override int GetHashCode() =>
        Value.GetHashCode();

    public DoubleValue ConvertToDoubleValue()
    {
        if (DoubleValue.TryParse(Value, null, out DoubleValue result))
            return result;

        return default;     // When not parsable interpreted as 0
    }

    public StringValue ConvertToStringValue() =>
        this;

    public bool Equals(IScratchType? other)
    {
        if (other is null)
            return false;

        StringValue otherStringValue = other.ConvertToStringValue();
        return Value.Equals(otherStringValue.Value);
    }

    public int CompareTo(IScratchType? other)
    {
        if (other is null)
            return 1;     // this is larger than null

        string otherString = other.ConvertToStringValue().Value;
        return Value.CompareTo(otherString);
    }

    public static bool operator ==(StringValue left, StringValue right) =>
        left.Equals(right);

    public static bool operator !=(StringValue left, StringValue right)=>
        !left.Equals(right);

    public static bool operator <(StringValue left, StringValue right)=>
        left.CompareTo(right) < 0;

    public static bool operator <=(StringValue left, StringValue right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >(StringValue left, StringValue right) =>
        left.CompareTo(right) > 0;

    public static bool operator >=(StringValue left, StringValue right) =>
        left.CompareTo(right) >= 0;

    public static implicit operator string(StringValue value) =>
        value.Value;

    private string GetDebuggerDisplay() =>
        Value;
}
