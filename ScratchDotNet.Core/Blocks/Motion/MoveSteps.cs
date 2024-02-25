using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.StageObjects;
using ScratchDotNet.Core.Types;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ScratchDotNet.Core.Blocks.Motion;

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
    [Input]
    public IValueProvider StepsProvider
    {
        get => _stepsProvider;
        set
        {
            ThrowAtRuntime();
            _stepsProvider = value;
        }
    }
    private IValueProvider _stepsProvider;

    private const string _constOpCode = "motion_movesteps";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="steps">The count of steps to move</param>
    /// <exception cref="ArgumentNullException"></exception>
    public MoveSteps(double steps) : this(steps, BlockHelpers.GenerateBlockId())
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
        _stepsProvider = new DoubleValue(steps);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="stepsProvider">The steps provider</param>
    /// <exception cref="ArgumentNullException"></exception>
    public MoveSteps(IValueProvider stepsProvider) : this(stepsProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The Id of this block</param>
    /// <param name="stepsProvider">The steps provider</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public MoveSteps(IValueProvider stepsProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(stepsProvider, nameof(stepsProvider));
        _stepsProvider = stepsProvider;
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockToken">The block to parse</param>
#pragma warning disable CS8618
    internal MoveSteps(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected internal override async Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        double radians = figure.Direction * (Math.PI / 180.0);

        double value = (await StepsProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        double rx = value * Math.Sin(radians);
        double ry = value * Math.Cos(radians);

        double x = figure.X + rx;
        double y = figure.Y + ry;
        figure.MoveTo(x, y);
    }

    private string GetDebuggerDisplay()
    {
        double value = StepsProvider.GetDefaultResult().ConvertToDoubleValue();
        return string.Format("Move steps: {0}", value);
    }
}
