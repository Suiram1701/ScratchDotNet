using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
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
    [Input("SECS")]
    public IValueProvider TimeProvider
    {
        get => _timeProvider;
        set
        {
            ThrowAtRuntime();
            _timeProvider = value;
        }
    }
    private IValueProvider _timeProvider;

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
        _timeProvider = new DoubleValue(time.TotalSeconds);
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
        _timeProvider = timeProvider;
    }

#pragma warning disable CS8618
    internal GlideBase(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected virtual string GetDebuggerDisplay()
    {
        double timeSeconds = TimeProvider.GetDefaultResult().ConvertToDoubleValue();
        return string.Format("Glide in {0}s to ", timeSeconds);
    }
}