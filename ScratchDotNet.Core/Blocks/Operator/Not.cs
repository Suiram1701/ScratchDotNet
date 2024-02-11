using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Operator;

/// <summary>
/// Invert a logical value
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Not : ValueOperatorBase, IBoolValueProvider
{
    public override event EventHandler<ValueChangedEventArgs> OnValueChanged
    {
        add => ValueProvider.OnValueChanged += value;
        remove => ValueProvider.OnValueChanged -= value;
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
    public Not(IBoolValueProvider valueProvider) : this(valueProvider, BlockHelpers.GenerateBlockId())
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

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (ValueProvider is null)
            return new BooleanValue(false);

        bool value = await ValueProvider.GetBooleanResultAsync(context, logger, ct);
        return new BooleanValue(!value);
    }

    private string GetDebuggerDisplay()
    {
        bool value = ValueProvider?.GetDefaultResult() as BooleanValue?
            ?? true;
        return string.Format("!{0}", value);
    }
}
