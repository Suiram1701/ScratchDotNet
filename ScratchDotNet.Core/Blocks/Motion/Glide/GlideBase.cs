using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Extensions;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion.Glide;

/// <summary>
/// A base implementation of read the seconds from the JToken
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract class GlideBase : ExecutionBlockBase
{
    /// <summary>
    /// The provider of the seconds the figure needs to move to the position
    /// </summary>
    public IValueProvider TimeProvider { get; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="opCode">The op code of the block that inherit from this</param>
    protected GlideBase(string opCode) : base(opCode)
    {
        TimeProvider = new Empty(DataType.Number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="time">The seconds the figure needes</param>
    /// <param name="opCode">The op code of this block</param>
    /// <param name="blockId">The Id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    protected GlideBase(TimeSpan time, string opCode, string blockId) : base(opCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(time, nameof(time));
        TimeProvider = new Result(time.TotalSeconds, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="timeProvider">The provider of the seconds the figure needs to move</param>
    /// <param name="opcode">The op code of this block</param>
    /// <param name="blockId">The Id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    protected GlideBase(IValueProvider timeProvider, string opcode, string blockId) : base(opcode, blockId)
    {
        ArgumentNullException.ThrowIfNull(timeProvider, nameof(timeProvider));

        TimeProvider = timeProvider;
        if (TimeProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.Number;
    }

    internal GlideBase(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        TimeProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.SECS") ?? new Empty(DataType.Number);
    }

    protected virtual string GetDebuggerDisplay()
    {
        double timeSeconds = TimeProvider.GetDefaultResult().ConvertToDoubleValue();
        return string.Format("Glide in {0}s to ", timeSeconds);
    }
}