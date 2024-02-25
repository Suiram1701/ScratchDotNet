using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Control;

/// <summary>
/// Repeats a chain of blocks a specific count
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Repeat : ExecutionBlockBase
{
    /// <summary>
    /// The provider of the value how many times the substack should be executed
    /// </summary>
    [Input]
    public IValueProvider TimesProvider { get; }

    /// <summary>
    /// The blocks that will be executed at invokation
    /// </summary>
    [Substack]
    public Substack Substack { get; }

    private const string _constOpCode = "control_repeat";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="times">How many times the stack should be executed (the value have to be greater or same than 0)</param>
    /// <param name="substack">The substack to execute</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Repeat(int times, IList<ExecutionBlockBase> substack) : this(times, substack, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="times">How many times the stack should be executed (the value have to be greater or same than 0)</param>
    /// <param name="substack">The substack to execute</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Repeat(int times, IList<ExecutionBlockBase> substack, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(times, nameof(times));
        if (times < 0)
            throw new ArgumentOutOfRangeException(nameof(times), times, "A value greater or same than 0 was expected.");
        ArgumentNullException.ThrowIfNull(substack, nameof(substack));

        TimesProvider = new DoubleValue(times);
        Substack = new(substack);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="timesProvider">The provider of the value that indicates how many times to executed the substack</param>
    /// <param name="substack">The substack to execute</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Repeat(IValueProvider timesProvider, IList<ExecutionBlockBase> substack) : this(timesProvider, substack, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="timesProvider">The provider of the value that indicates how many times to executed the substack</param>
    /// <param name="substack">The substack to execute</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Repeat(IValueProvider timesProvider, IList<ExecutionBlockBase> substack, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(timesProvider, nameof(timesProvider));
        ArgumentNullException.ThrowIfNull(substack, nameof(substack));

        TimesProvider = timesProvider;
        Substack = new(substack);
    }

#pragma warning disable CS8618
    internal Repeat(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double times = (await TimesProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        if (times < 0)
        {
            logger.LogWarning("The value that indicates how many times the substack should be executed have to be larger or same that 0. Value: {times}", times);
            return;
        }
        else if (times % 1 != 0)
        {
            double rounded = Math.Round(times, 0);
            logger.LogWarning("Unable to execute the substack by a count that is fractional. In cause of this a rounded count is used. Value: {times}; Rounded: {rounded}", times, rounded);
            times = rounded;
        }

        for (int i = 0; i < times; i++)
        {
            await Substack.InvokeAsync(context, logger, ct);

            if (ct.IsCancellationRequested)
                break;
        }
    }

    private string GetDebuggerDisplay()
    {
        int times = (int)Math.Round(TimesProvider.GetDefaultResult().ConvertToDoubleValue(), 0);
        return string.Format("Repeat {0} times; Substack: {1} blocks", times, Substack.Count);
    }
}
