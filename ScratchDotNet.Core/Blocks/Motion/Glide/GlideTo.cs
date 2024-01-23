﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.StageObjects;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion.Glide;

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
    /// <exception cref="ArgumentNullException"></exception>
    public GlideTo(TimeSpan time, SpecialTarget target) : this(time, target, BlockHelpers.GenerateBlockId())
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
        TargetProvider = new TargetReporter(target, TargetReporter.GlideToOpCode);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="time">The time the figure needs to move there</param>
    /// <param name="target">The figure to move to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideTo(TimeSpan time, IFigure target) : this(time, target, BlockHelpers.GenerateBlockId())
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
    /// <remarks>
    /// A target provider that implements <see cref="IConstProvider"/> is not supported. To provide a constant value you have use a constructor that takes an instance of <see cref="SpecialTarget"/> or <see cref="IFigure"/>
    /// </remarks>
    /// <param name="timeProvider">The provider of the time in seconds the figure needs to move there</param>
    /// <param name="targetProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GlideTo(IValueProvider timeProvider, IValueProvider targetProvider) : this(timeProvider, targetProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// A target provider that implements <see cref="IConstProvider"/> is not supported. To provide a constant value you have use a constructor that takes an instance of <see cref="SpecialTarget"/> or <see cref="IFigure"/>
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
            string message = string.Format(
                "A target provider that implements {0} is not supported. To provide a constant value you have use a constructor that takes an instance of {1} or {2}",
                nameof(IConstProvider),
                nameof(SpecialTarget),
                nameof(IFigure));
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
        if (context.Executor is not IFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        string target = (await TargetProvider.GetResultAsync(context, logger, ct)).GetStringValue();
        double timeSeconds = (await TimeProvider.GetResultAsync(context, logger, ct)).GetNumberValue();

        (double x, double y) = MotionHelpers.GetTargetPosition(target, context, logger);
        await figure.GlideToAsync(x, y, TimeSpan.FromSeconds(timeSeconds), ct);
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