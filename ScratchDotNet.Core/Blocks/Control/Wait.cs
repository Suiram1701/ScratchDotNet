using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks.Attributes;
using Scratch.Core.Blocks.Bases;
using Scratch.Core.Blocks.Interfaces;
using Scratch.Core.Blocks.Operator;
using Scratch.Core.Blocks.Operator.ConstProviders;
using Scratch.Core.Enums;
using Scratch.Core.Extensions;
using Scratch.Core.Types;
using System.Diagnostics;

namespace Scratch.Core.Blocks.Control;

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
    public IValueProvider DurationProvider { get; }

    private const string _constOpCode = "control_wait";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="duration">The time to wait</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Wait(TimeSpan duration) : this(duration, GenerateBlockId())
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

        DurationProvider = new Result(duration.TotalSeconds, true);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="durationProvider">The provider of the time in seconds to wait</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Wait(IValueProvider durationProvider) : this(durationProvider, GenerateBlockId())
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
        if (DurationProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.PositiveNumber;
    }

    internal Wait(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        DurationProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.DURATION") ?? new Empty(DataType.PositiveNumber);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double duration = (await DurationProvider.GetResultAsync(context, logger, ct)).GetNumberValue();

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
        double duration = DurationProvider.GetDefaultResult().GetNumberValue();
        return string.Format("Wait: {0}s", duration);
    }
}
