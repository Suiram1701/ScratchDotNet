﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks.Attributes;
using Scratch.Core.Blocks.Bases;
using Scratch.Core.Blocks.Interfaces;
using Scratch.Core.Blocks.Operator;
using Scratch.Core.Blocks.Operator.ConstProviders;
using Scratch.Core.Enums;
using Scratch.Core.Extensions;
using Scratch.Core.Types;
using System.Diagnostics;

namespace Scratch.Core.Blocks.Motion;

/// <summary>
/// Move the figure by a specified steps forward
/// </summary>
/// <remarks>
/// This can only executed from a figure
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class MoveSteps : ExecutionBlockBase
{
    /// <summary>
    /// The provider of steps how much steps the figure should do
    /// </summary>
    public IValueProvider StepsProvider { get; }

    private const string _constOpCode = "motion_movesteps";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public MoveSteps() : base(_constOpCode)
    {
        StepsProvider = new Empty(DataType.Number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="steps">The count of steps to move</param>
    /// <exception cref="ArgumentNullException"></exception>
    public MoveSteps(double steps) : this(steps, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The Id of this block</param>
    /// <param name="steps">The count of steps to move</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public MoveSteps(double steps, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(steps, nameof(steps));
        StepsProvider = new Result(steps, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The Id of this block</param>
    /// <param name="stepsProvider">The steps provider</param>
    /// <exception cref="ArgumentNullException"></exception>
    public MoveSteps(IValueProvider stepsProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(stepsProvider, nameof(stepsProvider));

        StepsProvider = stepsProvider;
        if (StepsProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.Number;
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockToken">The block to parse</param>
    internal MoveSteps(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        StepsProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.STEPS") ?? new Empty(DataType.Number);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Figure is null)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        double radians = context.Figure.Direction * (Math.PI / 180.0);

        double value = (await StepsProvider.GetResultAsync(context, logger, ct)).GetNumberValue();
        double rx = value * Math.Sin(radians);
        double ry = value * Math.Cos(radians);

        double x = context.Figure.X + rx;
        double y = context.Figure.Y + ry;
        context.Figure.MoveTo(x, y);
    }

    private string GetDebuggerDisplay()
    {
        double value = StepsProvider.GetDefaultResult().GetNumberValue();
        return string.Format("Move steps: {0}", value);
    }
}