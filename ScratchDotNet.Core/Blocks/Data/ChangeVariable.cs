using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
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
    [InputProvider]
    public IValueProvider ValueProvider { get; }

    private const string _constOpCode = "data_changevariableby";

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
        ValueProvider = new DoubleValue(value);
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
    }

#pragma warning disable CS8618
    internal ChangeVariable(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, Variable variable, ILogger logger, CancellationToken ct = default)
    {
        double value1 = (await ValueProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        double value2 = variable.Value.ConvertToDoubleValue();

        variable.Value = new DoubleValue(value1 + value2);
    }

    private string GetDebuggerDisplay()
    {
        double value = ValueProvider.GetDefaultResult().ConvertToDoubleValue();
        return string.Format("var {0} += {1}", VariableRef.VarName, value);
    }
}
