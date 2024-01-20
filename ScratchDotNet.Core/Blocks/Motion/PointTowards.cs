using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Providers.Interfaces;
using ScratchDotNet.Core.StageObjects;
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
    public PointTowards(SpecialTarget target) : this(target, BlockHelpers.GenerateBlockId())
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
    public PointTowards(IFigure target) : this(target, BlockHelpers.GenerateBlockId())
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
    /// A target provider that implements <see cref="IConstProvider"/> is not supported. To provide a constant value you have use a constructor that takes an instance of <see cref="SpecialTarget"/> or <see cref="IFigure"/>
    /// </remarks>
    /// <param name="targetProvider">The provider of the target figure name</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PointTowards(IValueProvider targetProvider) : this(targetProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// A target provider that implements <see cref="IConstProvider"/> is not supported. To provide a constant value you have use a constructor that takes an instance of <see cref="SpecialTarget"/> or <see cref="IFigure"/>
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
            string message = string.Format(
                "A target provider that implements {0} is not supported. To provide a constant value you have use a constructor that takes an instance of {1} or {2}",
                nameof(IConstProvider),
                nameof(SpecialTarget),
                nameof(IFigure));
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
        if (context.Executor is not IFigure figure)
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
                if (context.Services[typeof(IInputProviderService)] is not IInputProviderService provider)
                {
                    logger.LogCritical("Could not find any registered service that implement {interface}", nameof(IInputProviderService));

                    string message = string.Format("Could not find any registered implementation of {0}", nameof(IInputProviderService));
                    throw new InvalidOperationException(message);
                }

                Point mousePosition = provider.GetMousePosition();
                return GetFigureAngle(figure, mousePosition.X, mousePosition.Y);
            }
            ,
            _ => () =>
            {
                IFigure? targetFigure = context.Figures.FirstOrDefault(f => f.Name.Equals(target));
                if (targetFigure is null)     // Do not change direction when not found
                {
                    logger.LogWarning("Could not find any figure with name \"{name}\" on stage", target);
                    return figure.Direction;
                }

                return GetFigureAngle(figure, targetFigure.X, targetFigure.Y);
            }
        };
        double degree = degreeFunc();

        if (double.IsNaN(degree))     // Figure not found
        {
            logger.LogError("Cant find figure {figure} on stage", target);
            return;
        }

        figure.RotateTo(degree);
    }

    private static double GetFigureAngle(IFigure figure, double otherX, double otherY)
    {


        double dx = otherX - figure.X;
        double dy = otherY - figure.Y;

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
