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
    [Input("OPERAND")]
    public IBoolValueProvider ValueProvider
    {
        get => _valueProvider;
        set
        {
            ThrowAtRuntime();
            _valueProvider = value;
        }
    }
    private IBoolValueProvider _valueProvider;

    private const string _constOpCode = "operator_not";

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
        _valueProvider = valueProvider;
    }

#pragma warning disable CS8618
    internal Not(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        bool value = await ValueProvider.GetBooleanResultAsync(context, logger, ct);
        return new BooleanValue(!value);
    }

    private string GetDebuggerDisplay()
    {
        bool value = ValueProvider?.GetDefaultResult() as BooleanValue? ?? true;
        return string.Format("!{0}", value);
    }
}
