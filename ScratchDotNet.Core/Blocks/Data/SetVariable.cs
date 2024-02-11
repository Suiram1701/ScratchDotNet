using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Data;

/// <summary>
/// Sets the value of a variable
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class SetVariable : VariableExecutionBase
{
    /// <summary>
    /// The provider of the value to set
    /// </summary>
    public IValueProvider ValueProvider { get; }

    private const string _constOpCode = "data_setvariableto";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">A reference to the variable to set</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariable(VariableRef reference) : base(reference, _constOpCode)
    {
        ValueProvider = new Empty(DataType.String);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The value to put into the variable</param>
    /// <param name="reference">A reference to the variable to change</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariable(string value, VariableRef reference) : this(value, reference, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The value to put into the variable</param>
    /// <param name="reference">A reference to the variable to change</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariable(string value, VariableRef reference, string blockId) : base(reference, blockId, _constOpCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));
        ValueProvider = new Result(value);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="valueProvider">The provider of the value to put into the variable</param>
    /// <param name="reference">A reference to the variable to change</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariable(IValueProvider valueProvider, VariableRef reference) : this(valueProvider, reference, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="valueProvider">The provider of the value to put into the variable</param>
    /// <param name="reference">A reference to the variable to change</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariable(IValueProvider valueProvider, VariableRef reference, string blockId) : base(reference, blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(valueProvider, nameof(valueProvider));

        ValueProvider = valueProvider;
        if (ValueProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.String;
    }

    internal SetVariable(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ValueProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.VALUE") ?? new Empty(DataType.String);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, Variable variable, ILogger logger, CancellationToken ct = default)
    {
        IScratchType value = await ValueProvider.GetResultAsync(context, logger, ct);
        variable.Value = value;
    }

    private string GetDebuggerDisplay()
    {
        string value = ValueProvider.GetDefaultResult().ConvertToStringValue();
        return string.Format("var {0} = {1}", VariableRef.VarName, value);
    }
}
