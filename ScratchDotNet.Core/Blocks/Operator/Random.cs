using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Operator;

/// <summary>
/// Provides the generation of a random number between two numbers
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Random : ValueOperatorBase
{
    public override event Action OnValueChanged
    {
        add
        {
            MinProvider.OnValueChanged += value;
            MaxProvider.OnValueChanged += value;
        }
        remove
        {
            MinProvider.OnValueChanged -= value;
            MaxProvider.OnValueChanged -= value;
        }
    }

    /// <summary>
    /// The provider of the minimum value
    /// </summary>
    public IValueProvider MinProvider { get; }

    /// <summary>
    /// The provider of the maximum value
    /// </summary>
    public IValueProvider MaxProvider { get; }

    private const string _constOpCode = "operator_random";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public Random() : base(_constOpCode)
    {
        MinProvider = new Empty(DataType.Number);
        MaxProvider = new Empty(DataType.Number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="min">The minimum value</param>
    /// <param name="max">The maximum value</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Random(double min, double max) : this(min, max, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="min">The minimum value</param>
    /// <param name="max">The maximum value</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Random(double min, double max, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(min, nameof(min));
        ArgumentNullException.ThrowIfNull(max, nameof(max));

        MinProvider = new Result(min, false);
        MaxProvider = new Result(max, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="minProvider">The provider of the minimum value</param>
    /// <param name="maxProvider">The provider of the maximum value</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Random(IValueProvider minProvider, IValueProvider maxProvider) : this(minProvider, maxProvider, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="minProvider">The provider of the minimum value</param>
    /// <param name="maxProvider">The provider of the maximum value</param>
    /// <param name="blockId">The of of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Random(IValueProvider minProvider, IValueProvider maxProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(minProvider, nameof(minProvider));
        ArgumentNullException.ThrowIfNull(maxProvider, nameof(maxProvider));

        MinProvider = minProvider;
        if (MinProvider is IConstProvider minConstProvider)
            minConstProvider.DataType = DataType.Number;

        MaxProvider = maxProvider;
        if (MaxProvider is IConstProvider maxConstProvider)
            maxConstProvider.DataType = DataType.Number;
    }

    internal Random(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        MinProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.FROM")
            ?? new Empty(DataType.Number);
        MaxProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.TO")
            ?? new Empty(DataType.Number);
    }

    public override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        ScratchTypeBase minType = await MinProvider.GetResultAsync(context, logger, ct);
        double orgMin = minType.GetNumberValue();
        ScratchTypeBase maxType = await MaxProvider.GetResultAsync(context, logger, ct);
        double orgMax = maxType.GetNumberValue();

        double min = Math.Min(orgMin, orgMax);
        double max = Math.Max(orgMin, orgMax);

        double result;
        if (HasDecimal(min) || HasDecimal(max))
            result = System.Random.Shared.NextDouble() * (max - min);
        else
            result = System.Random.Shared.Next((int)min, (int)max);
        return new NumberType(result);
    }

    private static bool HasDecimal(double value) =>
        value % 1 != 0;

    private string GetDebuggerDisplay()
    {
        double min = MinProvider.GetDefaultResult().GetNumberValue();
        double max = MaxProvider.GetDefaultResult().GetNumberValue();

        return string.Format("Random: {0} - {1}", min, max);
    }
}
