using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Control;

/// <summary>
/// Waits a specified count of seconds
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Wait : ExecutionBlockBase
{
    /// <summary>
    /// The provider of the seconds to wait
    /// </summary>
    [Input]
    public IValueProvider DurationProvider { get; }

    private const string _constOpCode = "control_wait";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="duration">The time to wait</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Wait(TimeSpan duration) : this(duration, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="duration">The time to wait</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Wait(TimeSpan duration, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(duration, nameof(duration));
        if (duration.TotalSeconds < 0)
            throw new ArgumentOutOfRangeException(nameof(duration), duration, "The duration have to be greater or same that 0");

        DurationProvider = new DoubleValue(duration.TotalSeconds);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="durationProvider">The provider of the time in seconds to wait</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Wait(IValueProvider durationProvider) : this(durationProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="durationProvider">The provider of the time in seconds to wait</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Wait(IValueProvider durationProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(durationProvider, nameof(durationProvider));
        DurationProvider = durationProvider;
    }

#pragma warning disable CS8618
    internal Wait(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected internal override async Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double duration = (await DurationProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();

        if (duration < 0)
        {
            logger.LogWarning("A value greater or same than 0 was is required to wait.");
            return;
        }

        TimeSpan delay = TimeSpan.FromSeconds(duration);
        await Task.Delay(delay, ct);
    }

    private string GetDebuggerDisplay()
    {
        double duration = DurationProvider.GetDefaultResult().ConvertToDoubleValue();
        return string.Format("Wait: {0}s", duration);
    }
}
