using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ScratchDotNet.Core.Types;

/// <summary>
/// A double value that is dynamically convertible and comparable with other scratch types
/// </summary>
/// <param name="value">The initialized value</param>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct DoubleValue(double value) : IScratchType, IParsable<DoubleValue>, IValueProvider
{
    /// <inheritdoc/>
    /// <remarks>
    /// This will be never get called
    /// </remarks>
    public event EventHandler<ValueChangedEventArgs> OnValueChanged { add { } remove { } }

    /// <summary>
    /// The value this instance contains
    /// </summary>
    public readonly double Value = value;

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        Equals(obj as IScratchType);

    public override int GetHashCode() =>
        Value.GetHashCode();

    public DoubleValue ConvertToDoubleValue() =>
        this;

    public StringValue ConvertToStringValue() =>
        new(Value.ToString());

    public bool Equals(IScratchType? other)
    {
        if (other is null)
            return false;

        // Do string comparison because it when other isn't a double it would it interpreted as 0
        StringValue doubleString = ConvertToStringValue();
        return doubleString.Equals(other);
    }

    public int CompareTo(IScratchType? other)
    {
        if (other is null)
            return 1;     // this is larger than null

        StringValue doubleString = ConvertToStringValue();
        if (!double.TryParse(doubleString.Value, out double otherValue))
            return doubleString.CompareTo(other);     // Do a string comparison when other isn't a double

        return Value.CompareTo(otherValue);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out DoubleValue result)
    {
        if (double.TryParse(s, provider, out double doubleResult))
        {
            result = new(doubleResult);
            return true;
        }

        result = default;
        return false;
    }

    public static DoubleValue Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s, nameof(s));

        double value = double.Parse(s, provider);
        return new(value);
    }

    public Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default) =>
        Task.FromResult<IScratchType>(this);

    public static DoubleValue operator +(DoubleValue left, DoubleValue right)
    {
        double value = left.Value + right.Value;
        return new(value);
    }

    public static DoubleValue operator -(DoubleValue left, DoubleValue right)
    {
        double value = left.Value - right.Value;
        return new(value);
    }

    public static DoubleValue operator *(DoubleValue left, DoubleValue right)
    {
        double value = left.Value * right.Value;
        return new(value);
    }

    public static DoubleValue operator /(DoubleValue left, DoubleValue right)
    {
        double value = left.Value / right.Value;
        return new(value);
    }

    public static bool operator ==(DoubleValue left, DoubleValue right) =>
        left.Equals(right);

    public static bool operator !=(DoubleValue left, DoubleValue right) =>
        !left.Equals(right);

    public static bool operator <(DoubleValue left, DoubleValue right) =>
        left.CompareTo(right) < 0;

    public static bool operator <=(DoubleValue left, DoubleValue right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >(DoubleValue left, DoubleValue right) =>
        left.CompareTo(right) > 0;

    public static bool operator >=(DoubleValue left, DoubleValue right) =>
        left.CompareTo(right) >= 0;

    public static implicit operator double(DoubleValue value) =>
        value.Value;

    private string GetDebuggerDisplay() =>
        Value.ToString();
}
