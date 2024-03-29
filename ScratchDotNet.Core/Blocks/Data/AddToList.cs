﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Data;

/// <summary>
/// Appends a new item into a list
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class AddToList : ListExecutionBase
{
    /// <summary>
    /// The provider of the item to append
    /// </summary>
    [Input]
    public IValueProvider ItemProvider
    {
        get => _itemProvider;
        set
        {
            ThrowAtRuntime();
            _itemProvider = value;
        }
    }
    private IValueProvider _itemProvider;

    private const string _constOpCode = "data_addtolist";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item to append</param>
    /// <exception cref="ArgumentNullException"></exception>
    public AddToList(ListRef reference, IScratchType item) : this(reference, item, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item to append</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public AddToList(ListRef reference, IScratchType item, string blockId) : base(reference, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        _itemProvider = item.ConvertToStringValue();
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="itemProvider">The provider of the item to append</param>
    /// <exception cref="ArgumentNullException"></exception>
    public AddToList(ListRef reference, IValueProvider itemProvider) : this(reference, itemProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="itemProvider">The provider of the item to append</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public AddToList(ListRef reference, IValueProvider itemProvider, string blockId) : base(reference, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(itemProvider, nameof(itemProvider));
        _itemProvider = itemProvider;
    }

#pragma warning disable CS8618
    internal AddToList(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default)
    {
        IScratchType item = await ItemProvider.GetResultAsync(context, logger, ct);
        list.Values.Add(item);
    }

    private string GetDebuggerDisplay()
    {
        string itemString = ItemProvider.GetDefaultResult().ConvertToStringValue();
        return string.Format("List {0}.Add(\"{1}\")", ListRef.ListName, itemString);
    }
}
