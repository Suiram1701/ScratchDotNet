using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.StageObjects;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion.Glide;

/// <summary>
/// Glides a figur to specified coordinates
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class GlideToXY : GlideBase
{
    /// <summary>
    /// The X target position provider
    /// </summary>
    public IValueProvider TargetXProvider { get; }

    /// <summary>
    /// The Y target position provider
    /// </summary>
    public IValueProvider TargetYProvider { get; }

    private const string _constOpCode = "motion_glidesecstoxy";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public GlideToXY() : base(_constOpCode)
    {
        TargetXProvider = new Empty(DataType.Number);
        TargetYProvider = new Empty(DataType.Number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="time">The time the figure needs to move to the position</param>
    /// <param name="targetX">The target x position</param>
    /// <param name="targetY">The target y position</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideToXY(TimeSpan time, double targetX, double targetY) : this(time, targetX, targetY, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="time">The time the figure needs to move to the position</param>
    /// <param name="targetX">The target x position</param>
    /// <param name="targetY">The target y position</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideToXY(TimeSpan time, double targetX, double targetY, string blockId) : base(time, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(targetX, nameof(targetX));
        ArgumentNullException.ThrowIfNull(targetY, nameof(targetY));

        TargetXProvider = new Result(targetX, false);
        TargetYProvider = new Result(targetY, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="timeSecondsProvider">The provider of the seconds the figure needs to move there</param>
    /// <param name="targetXProvider">The provider of the target x position</param>
    /// <param name="targetYProvider">The provider of the target y position</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideToXY(IValueProvider timeSecondsProvider, IValueProvider targetXProvider, IValueProvider targetYProvider) : this(timeSecondsProvider, targetXProvider, targetYProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="timeSecondsProvider">The provider of the seconds the figure needs to move there</param>
    /// <param name="targetXProvider">The provider of the target x position</param>
    /// <param name="targetYProvider">The provider of the target y position</param>
    /// <param name="blockId">The id of the block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideToXY(IValueProvider timeSecondsProvider, IValueProvider targetXProvider, IValueProvider targetYProvider, string blockId) : base(timeSecondsProvider, _constOpCode, blockId)
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

    internal GlideToXY(string blockId, JToken blockToken) : base(blockId, blockToken)
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
        double timeSeconds = (await TimeProvider.GetResultAsync(context, logger, ct)).GetNumberValue();

        if (timeSeconds < 0)
        {
            logger.LogWarning("Could not glide the figure in a count of seconds that is less than 0 to the target position; Time: {time}s", timeSeconds);
            figure.MoveTo(targetX, targetY);
        }
        else
            await figure.GlideToAsync(targetX, targetY, TimeSpan.FromSeconds(timeSeconds), ct);
    }

    protected override string GetDebuggerDisplay()
    {
        double targetX = TargetXProvider.GetDefaultResult().GetNumberValue();
        double targetY = TargetYProvider.GetDefaultResult().GetNumberValue();

        string baseMessage = base.GetDebuggerDisplay();
        return baseMessage + string.Format("X: {0}; Y: {1}", targetX, targetY);
    }
}
