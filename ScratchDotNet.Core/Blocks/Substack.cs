using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Exceptions;
using ScratchDotNet.Core.Execution;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks;

/// <summary>
/// A list of blocks will be executed by a block
/// </summary>
[DebuggerDisplay($"Count = {{Count}}")]
public class Substack : Collection<ExecutionBlockBase>
{
    /// <summary>
    /// Indicates whether this substack is editable
    /// </summary>
    public bool Editable { get; private set; }

    /// <summary>
    /// Creates an empty substack
    /// </summary>
    public Substack() : base()
    {
        Editable = true;
    }

    /// <summary>
    /// Creates a new substack
    /// </summary>
    /// <param name="blocks">The blocks this substack contains</param>
    public Substack(IList<ExecutionBlockBase> blocks) : base(blocks)
    {
        Editable = true;
    }

    internal Substack(JToken blockToken, string jsonPath) : base()
    {
        Editable = false;

        string? startId = blockToken.SelectToken(jsonPath)?[1]?.Value<string>();
        if (string.IsNullOrEmpty(startId))     // The substack is empty
            return;

        string? nextId = startId;
        do
        {
            JToken? block = blockToken.Root[nextId];
            if (block is null)
                break;

            string? opCode = block["opcode"]?.Value<string>();
            if (!string.IsNullOrEmpty(opCode))
            {
                ExecutionBlockBase? blockInstance = ExecutionBlockCodeAttributeHelpers.GetFromOpCode(opCode, nextId, block);
                if (blockInstance is null)
                {
                    string message = string.Format("Could not find registered block for op code: {0}; block: {1}", opCode, nextId);
                    throw new Exception(message);
                }

                base.InsertItem(Count, blockInstance);     // Here the base method is used because the overridden method will throw an exception because 'Editable' is false
            }
            else
            {
                string message = string.Format("Unable to determine the op code of block {0}", nextId);
                throw new Exception(message);
            }

            nextId = block["next"]?.Value<string>();
        }
        while (!string.IsNullOrEmpty(nextId));
    }

    /// <summary>
    /// Invokes this substack 
    /// </summary>
    /// <param name="context">The context of the invocation</param>
    /// <param name="logger">The logger to use</param>
    /// <param name="ct">The cancellation token of the substack</param>
    /// <returns>The task that awaits the blocks</returns>
    internal async Task InvokeAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (Editable)
            Editable = false;

        if (Count == 0)
            return;

        using IDisposable? loggerScope = logger.BeginScope("Executing substack");

        foreach (ExecutionBlockBase block in this)
        {
            try
            {
                await block.ExecuteAsync(context, logger, ct);

                if (ct.IsCancellationRequested)
                    break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An occoured error happened while execution in block {block} while executing substack", block.BlockId);
            }
        }
    }

    private void ThrowIfNotEditable()
    {
        if (Editable)
            return;
        throw new NotEditableException("The substack isn't any longer editable at runtime or after the first execution.");
    }

    protected override void ClearItems()
    {
        ThrowIfNotEditable();
        base.ClearItems();
    }

    protected override void InsertItem(int index, ExecutionBlockBase item)
    {
        ThrowIfNotEditable();
        base.InsertItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
        ThrowIfNotEditable();
        base.RemoveItem(index);
    }

    protected override void SetItem(int index, ExecutionBlockBase item)
    {
        ThrowIfNotEditable();
        base.SetItem(index, item);
    }
}
