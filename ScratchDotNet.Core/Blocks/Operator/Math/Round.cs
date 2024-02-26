using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Operator.Math;

/// <summary>
/// Provides the rounding of a value
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Round : ValueOperatorBase
{
    public override event EventHandler<ValueChangedEventArgs> OnValueChanged
    {
        add => NumProvider.OnValueChanged += value;
        remove => NumProvider.OnValueChanged -= value;
    }

    /// <summary>
    /// The provider of the value to round
    /// </summary>
    [Input]
    public IValueProvider NumProvider
    {
        get => _numProvider;
        set
        {
            ThrowAtRuntime();
            _numProvider = value;
        }
    }
    private IValueProvider _numProvider;

    private const string _constOpCode = "operator_round";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="number">The number to round</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Round(double number) : this(number, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="number">The number to round</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Round(double number, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(number, nameof(number));
        _numProvider = new DoubleValue(number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="numProvider">The provider of the number to round</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Round(IValueProvider numProvider) : this(numProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="numProvider">The provider of the number to round</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Round(IValueProvider numProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(numProvider, nameof(NumProvider));
        _numProvider = numProvider;
    }

#pragma warning disable CS8618
    internal Round(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double number = (await NumProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        return new DoubleValue(System.Math.Round(number, 0));
    }

    private string GetDebuggerDisplay()
    {
        double number = NumProvider.GetDefaultResult().ConvertToDoubleValue();
        return string.Format("Round: {0}", number);
    }
}
