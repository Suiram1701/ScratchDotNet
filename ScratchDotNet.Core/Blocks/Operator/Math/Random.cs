using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Operator.Math;

/// <summary>
/// Provides the generation of a random number between two numbers
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Random : ValueOperatorBase
{
    public override event EventHandler<ValueChangedEventArgs> OnValueChanged
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
    /// <param name="min">The minimum value</param>
    /// <param name="max">The maximum value</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Random(double min, double max) : this(min, max, BlockHelpers.GenerateBlockId())
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

        MinProvider = new DoubleValue(min);
        MaxProvider = new DoubleValue(max);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="minProvider">The provider of the minimum value</param>
    /// <param name="maxProvider">The provider of the maximum value</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Random(IValueProvider minProvider, IValueProvider maxProvider) : this(minProvider, maxProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="minProvider">The provider of the minimum value</param>
    /// <param name="maxProvider">The provider of the maximum value</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Random(IValueProvider minProvider, IValueProvider maxProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(minProvider, nameof(minProvider));
        ArgumentNullException.ThrowIfNull(maxProvider, nameof(maxProvider));

        MinProvider = minProvider;
        MaxProvider = maxProvider;
    }

    internal Random(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        MinProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.FROM");
        MaxProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.TO");
    } 

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        DoubleValue orgMin = (await MinProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        DoubleValue orgMax = (await MaxProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();

        double min = System.Math.Min(orgMin, orgMax);
        double max = System.Math.Max(orgMin, orgMax);

        if (HasDecimal(min) || HasDecimal(max))     // Generate a random fractional value only when min or max is a fractional value otherwise generate a random integer value
        {
            // The random value have to get scaled with min and max because NextDouble() returns a value from 0.0d to 1.0d
            double scale = max - min;
            double result = min + (System.Random.Shared.NextDouble() * scale);

            return new DoubleValue(result);
        }
        else
        {
            double result = System.Random.Shared.Next((int)min, (int)max);
            return new DoubleValue(result);
        }
    }

    private static bool HasDecimal(double value) =>
        value % 1 != 0;

    private string GetDebuggerDisplay()
    {
        double min = MinProvider.GetDefaultResult().ConvertToDoubleValue();
        double max = MaxProvider.GetDefaultResult().ConvertToDoubleValue();

        return string.Format("Random: {0} - {1}", min, max);
    }
}
