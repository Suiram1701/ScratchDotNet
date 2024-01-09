using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Control;

/// <summary>
/// Provides logical execution of If and If else conditions
/// </summary>
[ExecutionBlockCode(_constIfOpCode)]
[ExecutionBlockCode(_constIfElseOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class IfElse : ExecutionBlockBase
{
    /// <summary>
    /// The provider of the condition to execute
    /// </summary>
    public IBoolValueProvider ConditionProvider { get; }

    /// <summary>
    /// The substack to execute at positive condition
    /// </summary>
    public ReadOnlyCollection<ExecutionBlockBase> Substack { get; }

    /// <summary>
    /// The substack to execute at negative condition
    /// </summary>
    public ReadOnlyCollection<ExecutionBlockBase>? ElseSubstack { get; }

    private const string _constIfOpCode = "control_if";
    private const string _constIfElseOpCode = "control_if_else";

    /// <summary>
    /// Creates a new instance of a if condition
    /// </summary>
    public IfElse() : base(_constIfOpCode)
    {
        ConditionProvider = new EmptyBool();
        Substack = new ReadOnlyCollection<ExecutionBlockBase>(new List<ExecutionBlockBase>(0));
    }

    /// <summary>
    /// Creates a new instance of a if condition
    /// </summary>
    /// <param name="conditionProvider">The required condition</param>
    /// <param name="substack">The substack to execute at positive condition</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentNullException"></exception>
    public IfElse(IBoolValueProvider conditionProvider, IList<ExecutionBlockBase> substack) : this(conditionProvider, substack, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance of a if condition
    /// </summary>
    /// <param name="conditionProvider">The required condition</param>
    /// <param name="substack">The substack to execute at positive condition</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public IfElse(IBoolValueProvider conditionProvider, IList<ExecutionBlockBase> substack, string blockId) : base(_constIfOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(conditionProvider, nameof(conditionProvider));
        ArgumentNullException.ThrowIfNull(substack, nameof(substack));

        ConditionProvider = conditionProvider;
        Substack = new ReadOnlyCollection<ExecutionBlockBase>(substack);
    }

    /// <summary>
    /// Creates a new instance of a if-else condition
    /// </summary>
    /// <param name="conditionProvider">The required condition</param>
    /// <param name="substack">The substack to execute at positive case</param>
    /// <param name="elseSubstack">The substack to execute at negative case</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentNullException"></exception>
    public IfElse(IBoolValueProvider conditionProvider, IList<ExecutionBlockBase> substack, IList<ExecutionBlockBase> elseSubstack) : this(conditionProvider, substack, elseSubstack, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance of a if-else condition
    /// </summary>
    /// <param name="conditionProvider">The required condition</param>
    /// <param name="substack">The substack to execute at positive case</param>
    /// <param name="elseSubstack">The substack to execute at negative case</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public IfElse(IBoolValueProvider conditionProvider, IList<ExecutionBlockBase> substack, IList<ExecutionBlockBase> elseSubstack, string blockId) : base(_constIfElseOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(conditionProvider, nameof(conditionProvider));
        ArgumentNullException.ThrowIfNull(substack, nameof(substack));
        ArgumentNullException.ThrowIfNull(elseSubstack, nameof(elseSubstack));

        ConditionProvider = conditionProvider;
        Substack = new ReadOnlyCollection<ExecutionBlockBase>(substack);
        ElseSubstack = new ReadOnlyCollection<ExecutionBlockBase>(elseSubstack);
    }

    internal IfElse(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ConditionProvider = BlockHelpers.GetBoolDataProvider(blockToken, "inputs.CONDITION");

        Substack = BlockHelpers.GetSubstack(blockToken, "inputs.SUBSTACK");
        if (_opCode == _constIfElseOpCode)
            ElseSubstack = BlockHelpers.GetSubstack(blockToken, "inputs.SUBSTACK2");
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        bool condition = false;
        if (ConditionProvider is not null)
            condition = (await ConditionProvider.GetResultAsync(context, logger, ct)).GetBoolValue();

        if (condition)
            await BlockHelpers.InvokeSubstackAsync(Substack, context, logger, ct);
        else
        {
            if (ElseSubstack?.Count > 0)
                await BlockHelpers.InvokeSubstackAsync(ElseSubstack, context, logger, ct);
        }
    }

    private string GetDebuggerDisplay()
    {
        string message = string.Format("if () {{ {0} blocks }}", Substack.Count);
        if (ElseSubstack is not null)
            message += string.Format(" else {{ {0} blocks }}", ElseSubstack.Count);

        return message;
    }
}
