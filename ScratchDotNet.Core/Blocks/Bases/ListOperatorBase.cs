using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Bases;

/// <summary>
/// The base for blocks that read data of variables
/// </summary>
public abstract class ListOperatorBase : ValueOperatorBase
{
    public sealed override event EventHandler<ValueChangedEventArgs>? OnValueChanged;

    /// <summary>
    /// The reference to the list of that the data should were read
    /// </summary>
    public ListRef ListRef { get; }

    private bool _delegateInitialized;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected ListOperatorBase(ListRef reference, string opcode) : base(opcode)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));
        ListRef = reference;
    }
    
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="blockId">The id of this block</param>
    /// <param name="opcode">The op code of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    protected ListOperatorBase(ListRef reference, string blockId, string opCode) : base(blockId, opCode)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));
        ListRef = reference;
    }

    protected internal ListOperatorBase(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ListRef = new(blockToken, "fields.LIST");
    }

    public sealed override Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        List? list = context.Executor.Lists.FirstOrDefault(list => list.Id.Equals(ListRef.ListId));
        if (list is null)
        {
            logger.LogError("Could not find list with id \"{id}\" and name \"{name}\"", ListRef.ListId, ListRef.ListName);
            return Task.FromResult<IScratchType>(new StringValue());
        }

        if (!_delegateInitialized)
        {
            list.OnValueChanged += List_OnValueChanged;

            logger.LogInformation("Value changed event of block {block} was successfully initialized", BlockId);
            _delegateInitialized = true;
        }

        return GetResultAsync(context, list, logger, ct);
    }

    /// <summary>
    /// The result providing method of this variable block
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="list">The list of this block</param>
    /// <param name="logger">The logger to use</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The async task</returns>
    protected abstract Task<IScratchType> GetResultAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default);

    private void List_OnValueChanged(object? s, ValueChangedEventArgs e) =>
        OnValueChanged?.Invoke(s, e);
}
