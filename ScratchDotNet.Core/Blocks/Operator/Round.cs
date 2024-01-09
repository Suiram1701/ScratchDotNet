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
/// Provides the rounding of a value
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Round : ValueOperatorBase
{
    public override event Action OnValueChanged
    {
        add => NumProvider.OnValueChanged += value;
        remove => NumProvider.OnValueChanged -= value;
    }

    /// <summary>
    /// The provider of the value to round
    /// </summary>
    public IValueProvider NumProvider { get; }

    private const string _constOpCode = "operator_round";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public Round() : base(_constOpCode)
    {
        NumProvider = new Empty(DataType.Number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="number">The number to round</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Round(double number) : this(number, GenerateBlockId())
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
        NumProvider = new Result(number, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="numProvider">The provider of the number to round</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Round(IValueProvider numProvider) : this(numProvider, GenerateBlockId())
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
        NumProvider = numProvider;
        if (NumProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.Number;
    }

    internal Round(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        NumProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.NUM")
            ?? new Empty(DataType.Number);
    }

    public override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double number = (await NumProvider.GetResultAsync(context, logger, ct)).GetNumberValue();
        return new NumberType(Math.Round(number, 0));
    }

    private string GetDebuggerDisplay()
    {
        double number = NumProvider.GetDefaultResult().GetNumberValue();
        return string.Format("Round: {0}", number);
    }
}
