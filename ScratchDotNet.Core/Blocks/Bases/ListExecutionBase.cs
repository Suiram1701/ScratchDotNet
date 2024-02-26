using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.Execution;

namespace ScratchDotNet.Core.Blocks.Bases;

/// <summary>
/// The base for blocks that work with lists
/// </summary>
public abstract class ListExecutionBase : ExecutionBlockBase
{
    /// <summary>
    /// The reference to the list
    /// </summary>
    public ListRef ListRef { get; }

    protected ListExecutionBase(ListRef reference, string opcode) : base(opcode)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));
        ListRef = reference;
    }

    protected ListExecutionBase(ListRef reference, string opCode, string blockId) : base(opCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));
        ListRef = reference;
    }

    protected internal ListExecutionBase(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ListRef = new(blockToken, "fields.LIST");
    }

    protected internal sealed override Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        List? list = context.Executor.Lists.FirstOrDefault(list => list.Id.Equals(ListRef.ListId));
        if (list is null)
        {
            logger.LogError("Could not find list with id \"{id}\" and name \"{name}\"", ListRef.ListId, ListRef.ListName);
            return Task.CompletedTask;
        }

        return ExecuteInternalAsync(context, list, logger, ct);
    }

    /// <summary>
    /// The execution of this list block
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="list">The list of this block</param>
    /// <param name="logger">The logger to use</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The async task</returns>
    protected abstract Task ExecuteInternalAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default);
}
