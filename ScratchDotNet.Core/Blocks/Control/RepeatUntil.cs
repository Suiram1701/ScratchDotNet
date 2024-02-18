using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Control;

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
    public Substack Substack { get; }

    private const string _constOpCode = "control_repeat_until";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="conditionProvider">The provider of the condition</param>
    /// <param name="substack">The substack to execute until the condition returns false</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RepeatUntil(IBoolValueProvider conditionProvider, IList<ExecutionBlockBase> substack) : this(conditionProvider, substack, BlockHelpers.GenerateBlockId())
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
        Substack = new(substack);
    }

    internal RepeatUntil(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ConditionProvider = BlockHelpers.GetBoolDataProvider(blockToken.Root, "inputs.CONDITION");
        Substack = new(blockToken, "inputs.SUBSTACK");
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        while (!await ConditionProvider.GetBooleanResultAsync(context, logger, ct))
        {
            await Substack.InvokeAsync(context, logger, ct);

            if (ct.IsCancellationRequested)
                break;
        }
    }

    private string GetDebuggerDisplay()
    {
        return string.Format("Repeat until; Substack: {0} blocks", Substack.Count);
    }
}
