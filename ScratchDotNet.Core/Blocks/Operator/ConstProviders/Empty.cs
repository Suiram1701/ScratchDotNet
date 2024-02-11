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
/// Provides an empty value
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Empty : IValueProvider, IConstProvider
{
    /// <inheritdoc/>
    /// <remarks>
    /// This will never be get called
    /// </remarks>
    public event EventHandler<ValueChangedEventArgs> OnValueChanged { add { } remove { } }

    public DataType DataType { get; set; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="dataType">The type of the empty value</param>
    /// <exception cref="ArgumentException"></exception>
    public Empty(DataType dataType)
    {
        if ((int)dataType >= 11)
            throw new ArgumentException("Result of the types 'Broadcast', 'Variable' und 'List' are not supported.", nameof(dataType));

        DataType = dataType;
    }

    public Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        IScratchType result = DataType switch
        {
            DataType.Number or
            DataType.PositiveNumber or
            DataType.Integer or
            DataType.PositiveNumber or
            DataType.Angle => new DoubleValue(0d),
            DataType.Color => throw new NotImplementedException(),
            DataType.String => new StringValue(string.Empty),
            _ => throw new NotSupportedException(string.Format("The specified data type {0} isn't supported.", DataType))
        };
        return Task.FromResult(result);
    }

    private string GetDebuggerDisplay()
    {
        return string.Format("Empty {0}", DataType.ToString());
    }
}
