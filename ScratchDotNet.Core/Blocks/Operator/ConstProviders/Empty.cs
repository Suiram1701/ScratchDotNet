using Microsoft.Extensions.Logging;
using Scratch.Core.Blocks.Interfaces;
using Scratch.Core.Enums;
using Scratch.Core.Types;
using Scratch.Core.Types.Bases;
using System.Diagnostics;
using System.Drawing;

namespace Scratch.Core.Blocks.Operator.ConstProviders;

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
    public event Action OnValueChanged { add { } remove { } }

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

    public Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        ScratchTypeBase result = DataType switch
        {
            DataType.Number or
            DataType.PositiveNumber or
            DataType.Integer or
            DataType.PositiveNumber or
            DataType.Angle => new NumberType(0d),
            DataType.Color => new ColorType(Color.Empty),
            DataType.String => new StringType(string.Empty),
            _ => throw new NotSupportedException(string.Format("The specified data type {0} isn't supported.", DataType))
        };
        return Task.FromResult(result);
    }

    private string GetDebuggerDisplay()
    {
        return string.Format("Empty {0}", DataType.ToString());
    }
}
