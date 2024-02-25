using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
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
    [Input]
    public IValueProvider ValueProvider { get; }

    private const string _constOpCode = "data_setvariableto";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">A reference to the variable to change</param>
    /// <param name="value">The value to put into the variable</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariable(VariableRef reference, IScratchType value) : this(reference, value, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">A reference to the variable to change</param>
    /// <param name="value">The value to put into the variable</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariable(VariableRef reference, IScratchType value, string blockId) : base(reference, blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        ValueProvider = value.ConvertToStringValue();
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">A reference to the variable to change</param>
    /// <param name="valueProvider">The provider of the value to put into the variable</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariable(VariableRef reference, IValueProvider valueProvider) : this(reference, valueProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">A reference to the variable to change</param>
    /// <param name="valueProvider">The provider of the value to put into the variable</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariable(VariableRef reference, IValueProvider valueProvider, string blockId) : base(reference, blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(valueProvider, nameof(valueProvider));
        ValueProvider = valueProvider;
    }

#pragma warning disable CS8618
    internal SetVariable(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
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
