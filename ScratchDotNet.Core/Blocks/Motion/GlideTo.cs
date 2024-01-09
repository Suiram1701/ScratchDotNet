using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks.Attributes;
using Scratch.Core.Blocks.Interfaces;
using Scratch.Core.Enums;
using Scratch.Core.Extensions;
using Scratch.Core.Figure;
using System.Diagnostics;

namespace Scratch.Core.Blocks.Motion;

/// <summary>
/// Slides a figure to a specified targetString
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class GlideTo : GlideBase
{
    /// <summary>
    /// The provider position to move to
    /// </summary>
    public IValueProvider TargetProvider { get; }

    private const string _constOpCode = "motion_glideto";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="time">The time the figure needs to move there</param>
    /// <param name="target">The special target to move</param>
    public GlideTo(TimeSpan time, SpecialTarget target) : this(time, target, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="time">The time the figure needs to glide to the targetString</param>
    /// <param name="target">The special target to move</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideTo(TimeSpan time, SpecialTarget target, string blockId) : base(time, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        if (target.HasAnyFlag())
            throw new ArgumentException("A enum instance with more than one flag is not allowed.", nameof(target));

        TargetProvider = new TargetReporter(target, TargetReporter.GlideToOpCode);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="time">The time the figure needs to move there</param>
    /// <param name="target">The figure to move to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideTo(TimeSpan time, IFigure target) : this(time, target, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="time">The time the figure needs to glide to the targetString</param>
    /// <param name="target">The figure to move to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideTo(TimeSpan time, IFigure target, string blockId) : base(time, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        TargetProvider = new TargetReporter(target, TargetReporter.GlideToOpCode);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="timeProvider">The provider of the time in seconds the figure needs to move there</param>
    /// <param name="targetProvider"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideTo(IValueProvider timeProvider, IValueProvider targetProvider) : this(timeProvider, targetProvider, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// If you want to provide a const target you have to use <see cref="TargetReporter"/> instead of <see cref="ResultOperator{String}"/> as <paramref name="targetProvider"/>
    /// </remarks>
    /// <param name="timeProvider">The provider of the time in seconds the figure needs to move there</param>
    /// <param name="targetProvider">The provider of the target to move to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideTo(IValueProvider timeProvider, IValueProvider targetProvider, string blockId) : base(timeProvider, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(targetProvider, nameof(targetProvider));

        TargetProvider = targetProvider;
        if (TargetProvider is IConstProvider)
        {
            string message = string.Format("A target provider that implements {0} is not supported.", nameof(IConstProvider));
            throw new ArgumentException(message, nameof(targetProvider));
        }
    }

    internal GlideTo(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        TargetProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.TO")
            ?? new TargetReporter(SpecialTarget.Random, TargetReporter.GlideToOpCode);     // random by default
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Figure is null)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        string target = (await TargetProvider.GetResultAsync(context, logger, ct)).GetStringValue();
        double timeSeconds = (await TimeProvider.GetResultAsync(context, logger, ct)).GetNumberValue();

        (double x, double y) = MotionHelpers.GetTargetPosition(target, context);
        await context.Figure.GlideToAsync(x, y, TimeSpan.FromSeconds(timeSeconds), ct);
    }

    protected override string GetDebuggerDisplay()
    {
        string target = TargetProvider.GetDefaultResult().GetStringValue();
        string targetString = target switch
        {
            "_random_" => "random position",
            "_mouse_" => "mouse position",
            _ => string.Format("figure {0}", target)
        };

        string baseMessage = base.GetDebuggerDisplay();
        return baseMessage + targetString;
    }
}
