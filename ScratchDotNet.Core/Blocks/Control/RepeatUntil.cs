using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks.Attributes;
using Scratch.Core.Blocks.Bases;
using Scratch.Core.Blocks.Interfaces;
using Scratch.Core.Blocks.Operator.ConstProviders;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Scratch.Core.Blocks.Control;

/// <summary>
/// Repeats the substack until the <see cref="ConditionProvider"/> returns <see langword="true"/>
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class RepeatUntil : ExecutionBlockBase
{
    /// <summary>
    /// The condition to check
    /// </summary>
    public IBoolValueProvider ConditionProvider { get; }

    /// <summary>
    /// The substack to execute
    /// </summary>
    public IReadOnlyCollection<ExecutionBlockBase> Substack { get; }

    private const string _constOpCode = "control_repeat_until";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public RepeatUntil() : base(_constOpCode)
    {
        ConditionProvider = new EmptyBool();
        Substack = new ReadOnlyCollection<ExecutionBlockBase>(new List<ExecutionBlockBase>(0));
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="substack">The substack to execute until the condition returns false</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RepeatUntil(IList<ExecutionBlockBase> substack) : base(_constOpCode)
    {
        ArgumentNullException.ThrowIfNull(substack, nameof(substack));

        ConditionProvider = new EmptyBool();
        Substack = new ReadOnlyCollection<ExecutionBlockBase>(substack);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="conditionProvider">The provider of the condition</param>
    /// <param name="substack">The substack to execute until the condition returns false</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RepeatUntil(IBoolValueProvider conditionProvider, IList<ExecutionBlockBase> substack) : this(conditionProvider, substack, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="conditionProvider">The provider of the condition</param>
    /// <param name="substack">The substack to execute until the condition returns false</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public RepeatUntil(IBoolValueProvider conditionProvider, IList<ExecutionBlockBase> substack, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(conditionProvider, nameof(conditionProvider));
        ArgumentNullException.ThrowIfNull(substack, nameof(substack));

        ConditionProvider = conditionProvider;
        Substack = new ReadOnlyCollection<ExecutionBlockBase>(substack);
    }

    internal RepeatUntil(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ConditionProvider = BlockHelpers.GetBoolDataProvider(blockToken.Root, "inputs.CONDITION");
        Substack = BlockHelpers.GetSubstack(blockToken, "inputs.SUBSTACK");
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (ConditionProvider is null)     // No condition
            return;

        while (!(await ConditionProvider.GetResultAsync(context, logger, ct)).GetBoolValue())
        {
            if (ct.IsCancellationRequested)
                break;
            await BlockHelpers.InvokeSubstackAsync(Substack, context, logger, ct);
        }
    }

    private string GetDebuggerDisplay()
    {
        return string.Format("Repeat until; Substack: {0} blocks", Substack.Count);
    }
}
