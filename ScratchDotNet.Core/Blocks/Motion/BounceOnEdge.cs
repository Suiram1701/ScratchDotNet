using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.StageObjects;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace ScratchDotNet.Core.Blocks.Motion;

/// <summary>
/// If the figure is at the border it get a bounce
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class BounceOnEdge : ExecutionBlockBase
{
    private const string _constOpCode = "motion_ifonedgebounce";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public BounceOnEdge() : this(BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    public BounceOnEdge(string blockId) : base(_constOpCode, blockId)
    {
    }

    internal BounceOnEdge(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    protected internal override Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IExecutableFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return Task.CompletedTask;
        }

        float figureX = Convert.ToSingle(figure.X);
        float figureY = Convert.ToSingle(figure.Y);
        Vector2 figureCenter = new(figureX, figureY);

        SizeF figureSize = figure.GetRenderedSize();
        SizeF stageSize = context.Stage.GetRenderedSize();

        float radian = Convert.ToSingle(figure.Direction) * (MathF.PI / 180);
        Matrix3x2 rotation = Matrix3x2.CreateRotation(radian, figureCenter);

        Vector2[] edgePoints =
        [
            new(-figureSize.Width / 2, -figureSize.Height / 2),
            new(figureSize.Width / 2, -figureSize.Height / 2),
            new(figureSize.Width / 2, figureSize.Height / 2),
            new(-figureSize.Width / 2, figureSize.Height / 2)

        ];
        for (int i = 0; i < edgePoints.Length; i++)
            edgePoints[i] = Vector2.Transform(edgePoints[i], rotation) + figureCenter;

        // adjust X and Y when out of stage
        double? targetX = null;
        double? targetY = null;

        double minX = edgePoints.Min(c => c.X);
        double maxX = edgePoints.Max(c => c.X);
        if (minX < -stageSize.Width)
            targetX = -stageSize.Width;
        else if (maxX > stageSize.Width)
            targetX = stageSize.Width;

        double minY = edgePoints.Min(d => d.Y);
        double maxY = edgePoints.Max(d => d.Y);
        if (minY < -stageSize.Height)
            targetY = -stageSize.Height;
        else if (maxY > stageSize.Height)
            targetY = stageSize.Height;

        figure.MoveTo(targetX ?? figure.X, targetY ?? figure.Y);
        return Task.CompletedTask;
    }

    private static string GetDebuggerDisplay()
    {
        return "Bounce on edge";
    }
}
