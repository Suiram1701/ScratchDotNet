using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.StageObjects;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion.Goto;

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
    /// <remarks>
    /// A target provider that implements <see cref="IConstProvider"/> is not supported. To provide a constant value you have use a constructor that takes an instance of <see cref="SpecialTarget"/> or <see cref="IFigure"/>
    /// </remarks>
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
    /// A target provider that implements <see cref="IConstProvider"/> is not supported. To provide a constant value you have use a constructor that takes an instance of <see cref="SpecialTarget"/> or <see cref="IFigure"/>
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
            string message = string.Format(
                "A target provider that implements {0} is not supported. To provide a constant value you have use a constructor that takes an instance of {1} or {2}",
                nameof(IConstProvider),
                nameof(SpecialTarget),
                nameof(IFigure));
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
        if (context.Executor is not IFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        string target = (await TargetProvider.GetResultAsync(context, logger, ct)).ConvertToStringValue();
        (double x, double y) = MotionHelpers.GetTargetPosition(target, context, logger);

        figure.MoveTo(x, y);
    }

    private string GetDebuggerDisplay()
    {
        string target = TargetProvider.GetDefaultResult().ConvertToStringValue();
        string targetString = target switch
        {
            "_random_" => "random position",
            "_mouse_" => "mouse position",
            _ => string.Format("figure {0}", target)
        };

        return string.Format("Goto: {0}", targetString);
    }
}
