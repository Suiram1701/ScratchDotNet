﻿using Microsoft.Extensions.Logging;
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

    private const int _stageHeight = 380;
    private const int _stageWidth = 500;

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
        if (context.Executor is not IFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return Task.CompletedTask;
        }

        float figureX = Convert.ToSingle(figure.X);
        float figureY = Convert.ToSingle(figure.Y);
        Vector2 figureCenter = new(figureX, figureY);

        float figureHeight = Convert.ToSingle(figure.Height);
        float figureWidth = Convert.ToSingle(figure.Width);
        SizeF figureSize = new(figureWidth, figureHeight);

        float radian = Convert.ToSingle(figure.Direction) * (MathF.PI / 180);
        Matrix3x2 rotation = Matrix3x2.CreateRotation(radian, figureCenter);

        Vector2[] corners = new[]
        {
            GetCornerVector(rotation.M11, figureSize) * new Vector2(),
            GetCornerVector(rotation.M12, figureSize),
            GetCornerVector(rotation.M21, figureSize),
            GetCornerVector(rotation.M22, figureSize),
        };

        double targetX = figure.X;
        double targetY = figure.Y;

        if (corners.Min(v => v.X) < -(_stageWidth / 2))
            targetX = -(_stageWidth / 2);
        else if (corners.Max(v => v.X) > _stageWidth / 2)
            targetX = _stageWidth / 2;

        if (corners.Min(v => v.Y) < -(_stageHeight / 2))
            targetY = -(_stageHeight / 2);
        else if (corners.Max(v => v.Y) > _stageHeight / 2)
            targetY = _stageHeight / 2;

        figure.MoveTo(targetX, targetY);
        return Task.CompletedTask;
    }

    /// <summary>
    /// The angle as radian
    /// </summary>
    /// <param name="angle">The angle</param>
    /// <param name="size">The size of the figure</param>
    /// <param name="e">The distance between the center and the corner</param>
    /// <returns>The vector</returns>
    private static Vector2 GetCornerVector(float angle, SizeF size)
    {
        (float x, float y) = MathF.SinCos(angle);
        return new Vector2(x * (size.Width / 2), y * (size.Height / 2));
    }

    private static string GetDebuggerDisplay()
    {
        return "Bounce on edge";
    }
}
