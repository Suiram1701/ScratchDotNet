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
using ScratchDotNet.Core.Types;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Data;

/// <summary>
/// Changes the value of a variable by a value
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ChangeVariable : VariableExecutionBase
{
    /// <summary>
    /// The provider of the value to change by
    /// </summary>
    public IValueProvider ValueProvider { get; }

    private const string _constOpCode = "data_changevariableby";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">A reference to A reference to the variable to change</param>
    public ChangeVariable(VariableRef reference) : base(reference, _constOpCode)
    {
        ValueProvider = new Empty(DataType.Number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The value to change by</param>
    /// <param name="reference">A reference to A reference to the variable to change</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangeVariable(double value, VariableRef reference) : this(value, reference, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The value to change by</param>
    /// <param name="reference">A reference to the variable to change</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangeVariable(double value, VariableRef reference, string blockId) : base(reference, blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        ValueProvider = new Result(value, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="valueProvider">The provider of the value to change by</param>
    /// <param name="reference">A reference to the variable to change</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangeVariable(IValueProvider valueProvider, VariableRef reference) : this(valueProvider, reference, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="valueProvider">The provider of the value to change by</param>
    /// <param name="reference">A reference to the variable to change</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangeVariable(IValueProvider valueProvider, VariableRef reference, string blockId) : base(reference, blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(valueProvider, nameof(valueProvider));

        ValueProvider = valueProvider;
        if (ValueProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.Number;
    }

    internal ChangeVariable(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ValueProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.VALUE") ?? new Empty(DataType.Number);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, Variable variable, ILogger logger, CancellationToken ct = default)
    {
        double value1 = (await ValueProvider.GetResultAsync(context, logger, ct)).GetNumberValue();
        double value2 = variable.Value.GetNumberValue();

        variable.Value = new NumberType(value1 + value2);
    }

    private string GetDebuggerDisplay()
    {
        double value = ValueProvider.GetDefaultResult().GetNumberValue();
        return string.Format("var {0} += {1}", VariableRef.VarName, value);
    }
}
