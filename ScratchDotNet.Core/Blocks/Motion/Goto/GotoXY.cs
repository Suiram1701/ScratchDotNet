using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.StageObjects;
using ScratchDotNet.Core.Types;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ScratchDotNet.Core.Blocks.Motion.Goto;

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
    [Input("X")]
    public IValueProvider TargetXProvider { get; }

    /// <summary>
    /// The target Y position provider
    /// </summary>
    [Input("Y")]
    public IValueProvider TargetYProvider { get; }

    private const string _constOpCode = "motion_gotoxy";

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

        TargetXProvider = new DoubleValue(targetX);
        TargetYProvider = new DoubleValue(targetY);
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
        TargetYProvider = targetYProvider;
    }

#pragma warning disable CS8618
    internal GotoXY(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        double targetX = (await TargetXProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        double targetY = (await TargetYProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();

        figure.MoveTo(targetX, targetY);
    }

    private string GetDebuggerDisplay()
    {
        double targetX = TargetXProvider.GetDefaultResult().ConvertToDoubleValue();
        double targetY = TargetYProvider.GetDefaultResult().ConvertToDoubleValue();
        return string.Format("Goto coordinates X: {0}; Y: {1}", targetX, targetY);
    }
}
