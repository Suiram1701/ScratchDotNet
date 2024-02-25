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

namespace ScratchDotNet.Core.Blocks.Motion;

/// <summary>
/// Set the figures rotation to a specified count of degrees
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class PointInDirection : ExecutionBlockBase
{
    /// <summary>
    /// The provider of the degree to set
    /// </summary>
    [Input]
    public IValueProvider DirectionProvider
    {
        get => _directionProvider;
        set
        {
            ThrowAtRuntime();
            _directionProvider = value;
        }
    }
    private IValueProvider _directionProvider;

    private const string _constOpCode = "motion_pointindirection";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="angle">The count of degrees to set</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public PointInDirection(double angle) : this(angle, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="angle">The count of degrees to set. This value have to be between 0° and 359°</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public PointInDirection(double angle, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(angle, nameof(angle));
        if (angle < 0 || angle >= 360)
            throw new ArgumentOutOfRangeException(nameof(angle), angle, "The count of degree have to be between 0° and 359°.");

        _directionProvider = new DoubleValue(angle);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="angleProvider">The provider of the count of degree to rotate</param>
    /// <exception cref="ArgumentNullException"></exception>
    public PointInDirection(IValueProvider angleProvider) : this(angleProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="angleProvider">The provider of the count of degrees to set</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PointInDirection(IValueProvider angleProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(angleProvider, nameof(angleProvider));
        _directionProvider = angleProvider;}

#pragma warning disable CS8618
    internal PointInDirection(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        double value = (await DirectionProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        if (double.IsNaN(value))
        {
            logger.LogWarning("The figure could not point to an empty value.");
            return;
        }

        figure.RotateTo(value);
    }

    private string GetDebuggerDisplay()
    {
        double value = DirectionProvider.GetDefaultResult().ConvertToDoubleValue();
        return string.Format("Rotate to: {0}°", value);
    }
}
