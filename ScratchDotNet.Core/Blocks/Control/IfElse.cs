using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
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
    [Input]
    public IBoolValueProvider ConditionProvider { get; }

    /// <summary>
    /// The substack to execute at positive condition
    /// </summary>
    [Substack]
    public Substack Substack { get; }

    /// <summary>
    /// The substack to execute at negative condition
    /// </summary>
    public Substack? ElseSubstack { get; }

    private const string _constIfOpCode = "control_if";
    private const string _constIfElseOpCode = "control_if_else";

    /// <summary>
    /// Creates a new instance of a if condition
    /// </summary>
    /// <param name="conditionProvider">The required condition</param>
    /// <param name="substack">The substack to execute at positive condition</param>
    /// <exception cref="ArgumentNullException"></exception>
    public IfElse(IBoolValueProvider conditionProvider, IList<ExecutionBlockBase> substack) : this(conditionProvider, substack, BlockHelpers.GenerateBlockId())
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
        Substack = new(substack);
    }

    /// <summary>
    /// Creates a new instance of a if-else condition
    /// </summary>
    /// <param name="conditionProvider">The required condition</param>
    /// <param name="substack">The substack to execute at positive case</param>
    /// <param name="elseSubstack">The substack to execute at negative case</param>
    /// <exception cref="ArgumentNullException"></exception>
    public IfElse(IBoolValueProvider conditionProvider, IList<ExecutionBlockBase> substack, IList<ExecutionBlockBase> elseSubstack) : this(conditionProvider, substack, elseSubstack, BlockHelpers.GenerateBlockId())
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
        Substack = new(substack);
        ElseSubstack = new(elseSubstack);
    }

#pragma warning disable CS8618
    internal IfElse(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
        if (_opCode == _constIfElseOpCode)
            ElseSubstack = new(blockToken, "inputs.SUBSTACK2");
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (await ConditionProvider.GetBooleanResultAsync(context, logger, ct))
            await Substack.InvokeAsync(context, logger, ct);
        else
        {
            if (ElseSubstack is not null)     // When ElseSubstack is null this block represents a simple if-condition and otherwise this represents a if-else-condition
                await ElseSubstack.InvokeAsync(context, logger, ct);
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
