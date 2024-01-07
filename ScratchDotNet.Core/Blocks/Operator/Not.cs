using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks.Attributes;
using Scratch.Core.Blocks.Bases;
using Scratch.Core.Blocks.Interfaces;
using Scratch.Core.Blocks.Operator.ConstProviders;
using Scratch.Core.Extensions;
using Scratch.Core.Types;
using Scratch.Core.Types.Bases;
using System.Diagnostics;

namespace Scratch.Core.Blocks.Operator;

/// <summary>
/// Invert a logical value
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Not : ValueOperatorBase, IBoolValueProvider
{
    public override event Action OnValueChanged
    {
        add => OnValueChanged += value;
        remove => OnValueChanged -= value;
    }

    /// <summary>
    /// The provider of the value to invert
    /// </summary>
    public IBoolValueProvider ValueProvider { get; }

    private const string _constOpCode = "operator_not";

    /// <summary>
    /// Creates a new instance that always returns true
    /// </summary>
    public Not() : this(new EmptyBool())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="valueProvider">The provider of the value to invert</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Not(IBoolValueProvider valueProvider) : this(valueProvider, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="valueProvider">The provider of the value to invert</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Not(IBoolValueProvider valueProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(valueProvider, nameof(valueProvider));
        ValueProvider = valueProvider;
    }

    internal Not(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ValueProvider = BlockHelpers.GetBoolDataProvider(blockToken, "inputs.OPERAND");
    }

    public override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (ValueProvider is null)
            return new BooleanType(false);

        bool value = (await ValueProvider.GetResultAsync(context, logger, ct)).GetBoolValue();
        return new BooleanType(!value);
    }

    private string GetDebuggerDisplay()
    {
        bool value = ValueProvider?.GetDefaultResult().GetBoolValue()
            ?? true;
        return string.Format("!{0}", value);
    }
}
