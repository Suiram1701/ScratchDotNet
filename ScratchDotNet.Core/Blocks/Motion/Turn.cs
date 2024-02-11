﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.StageObjects;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion;

/// <summary>
/// Rotates a figure by a specified value
/// </summary>
/// <remarks>
/// This can only executed from a figure
/// </remarks>
[ExecutionBlockCode(_constLeftOpCode)]
[ExecutionBlockCode(_constRightOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Turn : ExecutionBlockBase
{
    /// <summary>
    /// The direction to turn to
    /// </summary>
    public Direction Direction { get; }

    /// <summary>
    /// The provider of the value to turn
    /// </summary>
    public IValueProvider ValueProvider { get; }

    private const string _constLeftOpCode = "motion_turnleft";
    private const string _constRightOpCode = "motion_turnright";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="direction">The direction to rotate to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Turn(Direction direction) : base(GetOpCodeFromDirection(direction))
    {
        ArgumentNullException.ThrowIfNull(direction, nameof(direction));

        Direction = direction;
        ValueProvider = new Empty(DataType.Number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="direction">The direction to rotate to</param>
    /// <param name="value">The count of degrees to rotate</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Turn(Direction direction, double value) : this(direction, value, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="direction">The direction to rotate to</param>
    /// <param name="value">The count of degrees to rotate</param>
    /// <param name="blockId">The Id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Turn(Direction direction, double value, string blockId) : base(GetOpCodeFromDirection(direction), blockId)
    {
        ArgumentNullException.ThrowIfNull(direction, nameof(direction));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        Direction = direction;
        ValueProvider = new Result(value, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="direction">The direction to rotate to</param>
    /// <param name="valueProvider">The provider of the count of degrees to rotate</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Turn(Direction direction, IValueProvider valueProvider) : this(direction, valueProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="direction">The direction to rotate to</param>
    /// <param name="valueProvider">The provider of the count of degrees to rotate</param>
    /// <param name="blockId">The Id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Turn(Direction direction, IValueProvider valueProvider, string blockId) : base(GetOpCodeFromDirection(direction), blockId)
    {
        ArgumentNullException.ThrowIfNull(direction, nameof(direction));
        ArgumentNullException.ThrowIfNull(valueProvider, nameof(valueProvider));

        Direction = direction;
        ValueProvider = valueProvider;
        if (ValueProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.Number;
    }

    internal Turn(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ValueProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.DEGREES") ?? new Empty(DataType.Number);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        double value = (await ValueProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        double direction = figure.Direction + Direction switch
        {
            Direction.Left => -value,
            Direction.Right => value,
            _ => throw new NotSupportedException("The specified direction isn't supported.")
        };

        figure.RotateTo(direction);
    }

    private static string GetOpCodeFromDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Left => _constLeftOpCode,
            Direction.Right => _constRightOpCode,
            _ => throw new NotSupportedException("The specified direction isn't supported.")
        };
    }

    private string GetDebuggerDisplay()
    {
        string direction = Direction.ToString().ToLower();
        double value = ValueProvider.GetDefaultResult().ConvertToDoubleValue();

        return string.Format("Turn {0}: {1}°", direction, value);
    }
}
