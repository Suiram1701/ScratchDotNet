using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Extensions;
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
    public IValueProvider AngleProvider { get; }

    private const string _constOpCode = "motion_pointindirection";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public PointInDirection() : base(_constOpCode)
    {
        AngleProvider = new Empty(DataType.Angle);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="angle">The count of degrees to set</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public PointInDirection(double angle) : this(angle, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="angle">The count of degrees to set</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public PointInDirection(double angle, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(angle, nameof(angle));
        if (angle < -180 || angle > 180)
            throw new ArgumentOutOfRangeException(nameof(angle), angle, "The count of degree have to be between -180 and 180.");

        AngleProvider = new Result(angle);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="angleProvider">The provider of the count of degree to rotate</param>
    /// <exception cref="ArgumentNullException"></exception>
    public PointInDirection(IValueProvider angleProvider) : this(angleProvider, GenerateBlockId())
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

        AngleProvider = angleProvider;
        if (AngleProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.Angle;
    }

    internal PointInDirection(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        AngleProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.DIRECTION") ?? new Empty(DataType.Angle);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Figure is null)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        double value = (await AngleProvider.GetResultAsync(context, logger, ct)).GetNumberValue();
        if (double.IsNaN(value))
        {
            logger.LogWarning("The figure could not point to an empty value.");
            return;
        }

        context.Figure.RotateTo(value);
    }

    private string GetDebuggerDisplay()
    {
        double value = AngleProvider.GetDefaultResult().GetNumberValue();
        return string.Format("Rotate to: {0}°", value);
    }
}
