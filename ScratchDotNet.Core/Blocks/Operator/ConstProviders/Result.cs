﻿using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;
using System.Drawing;

namespace ScratchDotNet.Core.Blocks.Operator.ConstProviders;

/// <summary>
/// Provides an constant result
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Result : IValueProvider, IConstProvider
{
    /// <inheritdoc/>
    /// <remarks>
    /// This will never be called
    /// </remarks>
    public event EventHandler<ValueChangedEventArgs> OnValueChanged { add { } remove { } }

    /// <summary>
    /// The value to return
    /// </summary>
    public IScratchType Value { get; }

    public DataType DataType { get; set; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The valkue to provide</param>
    /// <param name="dataType">The type of the value</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Result(IScratchType value, DataType dataType)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        ArgumentNullException.ThrowIfNull(dataType, nameof(dataType));

        if (value is BooleanValue)
        {
            string message = string.Format("Result of the type {0} are not supported.", nameof(BooleanValue));
            throw new ArgumentException(message, nameof(value));
        }

        if ((int)dataType >= 11)
            throw new ArgumentException("Result of the types 'Broadcast', 'Variable' und 'List' are not supported.", nameof(dataType));

        Value = value;
        DataType = dataType;
    }

    /// <summary>
    /// Creates a new instance that provides a number
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="positive"><see langword="true"/> when the value have to be greater or same than 0</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Result(double value, bool positive)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        ArgumentNullException.ThrowIfNull(positive, nameof(positive));
        if (positive && value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), value, "When 'positive' is true the 'value' have to be greater or same than 0.");

        DataType = positive
            ? DataType.PositiveNumber
            : DataType.Number;
        Value = new DoubleValue(value);
    }

    /// <summary>
    /// Creates a new instance that provides an integer
    /// </summary>
    /// <param name="value"></param>
    public Result(int value) : this(value, false)
    {
    }

    /// <summary>
    /// Creates a new instance that provides a positive integer
    /// </summary>
    /// <param name="value">The value</param>
    public Result(uint value) : this(value, true)
    {
    }

    /// <summary>
    /// Creates a new instance that provides an integer
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="positive"><see langword="true"/> when the value have to be greater or same than 0</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Result(int value, bool positive)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        ArgumentNullException.ThrowIfNull(positive, nameof(positive));
        if (positive && value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), value, "When 'positive' is true the 'value' have to be greater or same than 0.");

        DataType = positive
            ? DataType.PositiveInteger
            : DataType.Integer;
        Value = new DoubleValue(value);
    }

    /// <summary>
    /// Creates a new instance that provides a angle
    /// </summary>
    /// <param name="value">The count of degree</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Result(double value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        if (value < -180 || value > 180)
            throw new ArgumentOutOfRangeException(nameof(value), value, "The count of degree have to be between -180 and 180.");

        DataType = DataType.Angle;
        Value = new DoubleValue(value);
    }

    /// <summary>
    /// Creates a new instance that provides a value
    /// </summary>
    /// <param name="value">The value</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Result(string value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        DataType = DataType.String;
        Value = new StringValue(value);
    }

    public Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default) =>
        Task.FromResult(Value);

    private string GetDebuggerDisplay()
    {
        return string.Format("const {0}: {1}", DataType.ToString(), Value?.ToString() ?? "null");
    }
}
