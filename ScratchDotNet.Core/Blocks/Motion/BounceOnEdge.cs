using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.StageObjects;
using ScratchDotNet.Core.StageObjects.Assets;
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

        PointF figurePosition = new(Convert.ToSingle(figure.X), Convert.ToSingle(figure.Y));
        SizeF figureSize = figure.GetRenderedSize();
        Vector2 figureRotationCenter = (figureSize / 2).ToVector2();

        // use the by the costume specified rotation center
        CostumeAsset costume = figure.Costumes.ElementAt(figure.CurrentCostum);
        Vector2 costumeRotationCenter = new(Convert.ToSingle(costume.RotationCenterX), Convert.ToSingle(costume.RotationCenterY));
        Vector2 rotationCenterOffset = costumeRotationCenter - figureRotationCenter;

        Vector2[] edgePoints =
        [
            new(-figureSize.Width / 2, -figureSize.Height / 2),
            new(figureSize.Width / 2, -figureSize.Height / 2),
            new(figureSize.Width / 2, figureSize.Height / 2),
            new(-figureSize.Width / 2, figureSize.Height / 2)
        ];

        float radian = Convert.ToSingle(figure.Direction) * (MathF.PI / 180);
        Matrix3x2 rotation = Matrix3x2.CreateRotation(radian, rotationCenterOffset);

        for (int i = 0; i < edgePoints.Length; i++)
            edgePoints[i] = Vector2.Transform(edgePoints[i], rotation) + figurePosition.ToVector2();

        SizeF stageSize = context.Stage.GetRenderedSize() / 2;

        /* The X and Y position will be adjusted so that the corner sits exactly on the stage border.
         * Explanation:
         *    1. Get the edge points with the smallest/largest X/Y coordinate
         *    2. When any of them is out of the stage border they will get adjusted
         *    3. Calculate the X/Y distance between the figure position and the corner
         *    4. Add the distance to the stage border coordinate. That returns the X/Y position for the figure where the corner is exacly on the border
         */
        double targetX;
        double minX = edgePoints.Min(c => c.X);
        double maxX = edgePoints.Max(c => c.X);
        if (minX < -stageSize.Width)
            targetX = -stageSize.Width + (figurePosition.X - minX);
        else if (maxX > stageSize.Width)
            targetX = stageSize.Width + (figurePosition.X - maxX);
        else
            targetX = figure.X;

        double targetY;
        double minY = edgePoints.Min(d => d.Y);
        double maxY = edgePoints.Max(d => d.Y);
        if (minY < -stageSize.Height)
            targetY = -stageSize.Height + (figurePosition.Y - minY);
        else if (maxY > stageSize.Height)
            targetY = stageSize.Height + (figurePosition.Y - maxY);
        else
            targetY = figure.Y;

        figure.MoveTo(targetX, targetY);
        return Task.CompletedTask;
    }

    private static string GetDebuggerDisplay()
    {
        return "Bounce on edge";
    }
}
