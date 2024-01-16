using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion;

/// <summary>
/// Changes the position of the figure by a specified kind and value
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[ExecutionBlockCode(_constAxOpCode)]
[ExecutionBlockCode(_constRxOpCode)]
[ExecutionBlockCode(_constAyOpCode)]
[ExecutionBlockCode(_constRyOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ChangePosition : ExecutionBlockBase
{
    /// <summary>
    /// The provider of the value to change by
    /// </summary>
    public IValueProvider ValueProvider { get; }

    /// <summary>
    /// The kind of the position change
    /// </summary>
    public PositionChangeKind ChangeKind { get; }

    private const string _constAxOpCode = "motion_setx";
    private const string _constRxOpCode = "motion_changexby";
    private const string _constAyOpCode = "motion_sety";
    private const string _constRyOpCode = "motion_changeyby";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="kind">The kind how the position of the figure should be changed</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangePosition(PositionChangeKind kind) : base(GetOpCodeFromKind(kind))
    {
        ArgumentNullException.ThrowIfNull(kind, nameof(kind));

        ChangeKind = kind;
        ValueProvider = new Empty(DataType.Number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="kind">The kind how the position of the figure should be changed</param>
    /// <param name="value">The value to change by</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangePosition(PositionChangeKind kind, double value) : this(kind, value, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="kind">The kind how the position of the figure should be changed</param>
    /// <param name="value">The value to change by</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangePosition(PositionChangeKind kind, double value, string blockId) : base(GetOpCodeFromKind(kind), blockId)
    {
        ArgumentNullException.ThrowIfNull(kind, nameof(kind));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        ChangeKind = kind;
        ValueProvider = new Result(value, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="kind">The kind how the position of the figure should be changed</param>
    /// <param name="valueProvider">The provider of the value to change by</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangePosition(PositionChangeKind kind, IValueProvider valueProvider) : this(kind, valueProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="kind">The kind how the position of the figure should be changed</param>
    /// <param name="valueProvider">The provider of the value to change by</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangePosition(PositionChangeKind kind, IValueProvider valueProvider, string blockId) : base(GetOpCodeFromKind(kind), blockId)
    {
        ArgumentNullException.ThrowIfNull(kind, nameof(kind));
        ArgumentNullException.ThrowIfNull(valueProvider, nameof(valueProvider));

        ChangeKind = kind;
        ValueProvider = valueProvider;
        if (ValueProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.Number;
    }

    internal ChangePosition(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ChangeKind = _opCode switch
        {
            _constAxOpCode => PositionChangeKind.X,
            _constRxOpCode => PositionChangeKind.DX,
            _constAyOpCode => PositionChangeKind.Y,
            _constRyOpCode => PositionChangeKind.DY,
            _ => throw new NotSupportedException("The specified position change kind isn't supported.")
        };

        string jsonPath = string.Format("inputs.{0}", ChangeKind.ToString());
        ValueProvider = BlockHelpers.GetDataProvider(blockToken, jsonPath) ?? new Empty(DataType.Number);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Figure is null)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        double value = (await ValueProvider.GetResultAsync(context, logger, ct)).GetNumberValue();

        double cx = context.Figure.X;
        double cy = context.Figure.Y;
        switch (ChangeKind)
        {
            case PositionChangeKind.X:
                context.Figure.MoveTo(value, cy);
                break;
            case PositionChangeKind.DX:
                context.Figure.MoveTo(cx + value, cy);
                break;
            case PositionChangeKind.Y:
                context.Figure.MoveTo(cx, value);
                break;
            case PositionChangeKind.DY:
                context.Figure.MoveTo(cx, cy + value);
                break;
        }

        return;
    }

    /// <summary>
    /// Returns the with the <see cref="PositionChangeKind"/> associated op code
    /// </summary>
    /// <param name="kind">The kind</param>
    /// <returns>The op code</returns>
    private static string GetOpCodeFromKind(PositionChangeKind kind)
    {
        return kind switch
        {
            PositionChangeKind.X => _constAxOpCode,
            PositionChangeKind.DX => _constRxOpCode,
            PositionChangeKind.Y => _constAyOpCode,
            PositionChangeKind.DY => _constRyOpCode,
            _ => throw new NotSupportedException("The specified position change kind isn't supported.")
        };
    }

    private string GetDebuggerDisplay()
    {
        string kind = ChangeKind switch
        {
            PositionChangeKind.X => "absolute x",
            PositionChangeKind.DX => "relative x",
            PositionChangeKind.Y => "absolute y",
            PositionChangeKind.DY => "relative y",
            _ => "unknown"
        };
        double value = ValueProvider.GetDefaultResult().GetNumberValue();

        return string.Format("Change {0} position with {1}", kind, value);
    }
}
