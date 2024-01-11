using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Figure;
using System.Diagnostics;
using System.Drawing;
using Random = System.Random;

namespace ScratchDotNet.Core.Blocks.Motion;

/// <summary>
/// Rotates the figure toward a target
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class PointTowards : ExecutionBlockBase
{
    /// <summary>
    /// The provider target to rotate to
    /// </summary>
    public IValueProvider TargetProvider { get; }

    private const string _constOpCode = "motion_pointtowards";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The special target of the rotation</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PointTowards(SpecialTarget target) : this(target, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The special target of the rotation</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PointTowards(SpecialTarget target, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        TargetProvider = new TargetReporter(target, TargetReporter.PointTowardsOpCode);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The target figure where the figure have to rotate to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public PointTowards(IFigure target) : this(target, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The target figure where the figure have to rotate to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PointTowards(IFigure target, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        TargetProvider = new TargetReporter(target, TargetReporter.PointTowardsOpCode);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// If you want to provide a constant target you have to use <see cref="TargetReporter"/> instead of <see cref="Result"/> at <paramref name="targetProvider"/> with <see cref="TargetReporter.PointTowardsOpCode"/> as op code
    /// </remarks>
    /// <param name="targetProvider">The provider of the target figure name</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PointTowards(IValueProvider targetProvider) : this(targetProvider, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// If you want to provide a constant target you have to use <see cref="TargetReporter"/> instead of <see cref="Result"/> at <paramref name="targetProvider"/> with <see cref="TargetReporter.PointTowardsOpCode"/> as op code
    /// </remarks>
    /// <param name="targetProvider">The provider of the target figure name</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PointTowards(IValueProvider targetProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(targetProvider, nameof(targetProvider));

        TargetProvider = targetProvider;
        if (TargetProvider is IConstProvider)
        {
            string message = string.Format("A target provider that implements {0} is not supported.", nameof(IConstProvider));
            throw new ArgumentException(message, nameof(targetProvider));
        }
    }

    internal PointTowards(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        TargetProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.TOWARDS")
            ?? new TargetReporter(SpecialTarget.Random, TargetReporter.PointTowardsOpCode);     // Random position as default
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Figure is null)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        string target = (await TargetProvider.GetResultAsync(context, logger, ct)).GetStringValue();
        Func<double> degreeFunc = target switch
        {
            "_random_" => () => Random.Shared.Next(1, 359),
            "_mouse_" => () =>
            {
                Point mousePosition = context.PhysicalDataProvider.MousePosition();
                return GetFigureAngle(context, mousePosition.X, mousePosition.Y);
            }
            ,
            _ => () =>
            {
                IFigure? figure = context.Figures.FirstOrDefault(f => f.Name == target);
                if (figure is null)
                    return double.NaN;

                return GetFigureAngle(context, figure.X, figure.Y);
            }
        };
        double degree = degreeFunc();

        if (double.IsNaN(degree))     // Figure not found
        {
            logger.LogError("Cant find figure {figure} on stage", target);
            return;
        }

        context.Figure.RotateTo(degree);
    }

    private static double GetFigureAngle(ScriptExecutorContext context, double otherX, double otherY)
    {
        double dx = otherX - context.Figure!.X;
        double dy = otherY - context.Figure.Y;

        double angleRadians = Math.Atan2(dy, dx);
        return angleRadians * (180 / Math.PI);
    }

    private string GetDebuggerDisplay()
    {
        string target = TargetProvider.GetDefaultResult().GetStringValue();
        string targetString = target switch
        {
            "_random_" => "random direction",
            "_mouse_" => "mouse position",
            _ => string.Format("figure {0}", target)
        };

        return string.Format("Rotate to: {0}", targetString);
    }
}
