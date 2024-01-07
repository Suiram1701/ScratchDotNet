using Microsoft.Extensions.Logging;
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
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Turn(Direction direction) : base(GetOpCodeFromDirection(direction))
    {
        ArgumentNullException.ThrowIfNull(direction, nameof(direction));
        if (direction.HasAnyFlag())
            throw new ArgumentException("A enum instance with more than one flag is not allowed.", nameof(direction));

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
    public Turn(Direction direction, double value) : this(direction, value, GenerateBlockId())
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
        if (direction.HasAnyFlag())
            throw new ArgumentException("A enum instance with more than one flag is not allowed.", nameof(direction));
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
    public Turn(Direction direction, IValueProvider valueProvider) : this(direction, valueProvider, GenerateBlockId())
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
        if (direction.HasAnyFlag())
            throw new ArgumentException("A enum instance with more than one flag is not allowed.", nameof(direction));
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
        if (context.Figure is null)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        double value = (await ValueProvider.GetResultAsync(context, logger, ct)).GetNumberValue();
        context.Figure.Direction += Direction switch
        {
            Direction.Left => -value,
            Direction.Right => value,
            _ => throw new NotSupportedException("The specified direction isn't supported.")
        };
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
        double value = ValueProvider.GetDefaultResult().GetNumberValue();

        return string.Format("Turn {0}: {1}°", direction, value);
    }
}
