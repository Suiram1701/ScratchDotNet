using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Figure;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion;

/// <summary>
/// Moves a figure to a specified position
/// </summary>
/// <remarks>
/// This can only executed from a figure
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Goto : ExecutionBlockBase
{
    /// <summary>
    /// The position to move to
    /// </summary>
    public IValueProvider TargetProvider { get; }

    private const string _constOpCode = "motion_goto";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The special target where the figure should go to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(SpecialTarget target) : this(target, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The special target where the figure should go to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(SpecialTarget target, string blockId) : this(MotionHelpers.GetTargetString(target), blockId)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        TargetProvider = new TargetReporter(target, TargetReporter.GotoOpCode);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The target figure where the figure should go to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(IFigure target) : this(target, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The target figure where the figure should go to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(IFigure target, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        TargetProvider = new TargetReporter(target, TargetReporter.GotoOpCode);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="targetProvider">The provider of the target figure</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(IValueProvider targetProvider) : this(targetProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// If you want to provide a constant target you have to use <see cref="TargetReporter"/> instead of <see cref="Result"/> at <paramref name="targetProvider"/> with <see cref="TargetReporter.GotoOpCode"/> as op code
    /// </remarks>
    /// <param name="targetProvider">The provider of the target figure</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(IValueProvider targetProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(targetProvider, nameof(targetProvider));

        TargetProvider = targetProvider;
        if (targetProvider is IConstProvider)
        {
            string message = string.Format("A target provider that implements {0} is not supported.", nameof(IConstProvider));
            throw new ArgumentException(message, nameof(targetProvider));
        }
    }

    internal Goto(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        TargetProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.TO")
            ?? new TargetReporter(SpecialTarget.Random, TargetReporter.GotoOpCode);     // Random position by default
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Figure is null)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        string target = (await TargetProvider.GetResultAsync(context, logger, ct)).GetStringValue();
        (double x, double y) = MotionHelpers.GetTargetPosition(target, context);

        context.Figure.MoveTo(x, y);
    }

    private string GetDebuggerDisplay()
    {
        string target = TargetProvider.GetDefaultResult().GetStringValue();
        string targetString = target switch
        {
            "_random_" => "random position",
            "_mouse_" => "mouse position",
            _ => string.Format("figure {0}", target)
        };

        return string.Format("Goto: {0}", targetString);
    }
}
