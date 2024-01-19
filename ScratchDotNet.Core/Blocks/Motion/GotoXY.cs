using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.StageObjects;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion;

/// <summary>
/// Moves the figure instandly to a a specified point
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class GotoXY : ExecutionBlockBase
{
    /// <summary>
    /// The target X position provider
    /// </summary>
    public IValueProvider TargetXProvider { get; }

    /// <summary>
    /// The target Y position provider
    /// </summary>
    public IValueProvider TargetYProvider { get; }

    private const string _constOpCode = "motion_gotoxy";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public GotoXY() : base(_constOpCode)
    {
        TargetXProvider = new Empty(DataType.Number);
        TargetYProvider = new Empty(DataType.Number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="targetX">The target x position</param>
    /// <param name="targetY">The target y position</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GotoXY(double targetX, double targetY) : this(targetX, targetY, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="targetX">The target x position</param>
    /// <param name="targetY">The target y position</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public GotoXY(double targetX, double targetY, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(targetX, nameof(targetX));
        ArgumentNullException.ThrowIfNull(targetY, nameof(targetY));

        TargetXProvider = new Result(targetX, false);
        TargetYProvider = new Result(targetY, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="targetXProvider">The provider of the target x position</param>
    /// <param name="targetYProvider">The provider of the target y position</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GotoXY(IValueProvider targetXProvider, IValueProvider targetYProvider) : this(targetXProvider, targetYProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="targetXProvider">The provider of the x position</param>
    /// <param name="targetYProvider">The provider of the y position</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public GotoXY(IValueProvider targetXProvider, IValueProvider targetYProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(targetXProvider, nameof(targetXProvider));
        ArgumentNullException.ThrowIfNull(targetYProvider, nameof(targetYProvider));

        TargetXProvider = targetXProvider;
        if (TargetXProvider is IConstProvider constXProvider)
            constXProvider.DataType = DataType.Number;

        TargetYProvider = targetYProvider;
        if (TargetYProvider is IConstProvider constYProvider)
            constYProvider.DataType = DataType.Number;
    }

    internal GotoXY(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        TargetXProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.X") ?? new Empty(DataType.Number);
        TargetYProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.Y") ?? new Empty(DataType.Number);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        double targetX = (await TargetXProvider.GetResultAsync(context, logger, ct)).GetNumberValue();
        double targetY = (await TargetYProvider.GetResultAsync(context, logger, ct)).GetNumberValue();

        figure.MoveTo(targetX, targetY);
    }

    private string GetDebuggerDisplay()
    {
        double targetX = TargetXProvider.GetDefaultResult().GetNumberValue();
        double targetY = TargetYProvider.GetDefaultResult().GetNumberValue();
        return string.Format("Goto coordinates X: {0}; Y: {1}", targetX, targetY);
    }
}
