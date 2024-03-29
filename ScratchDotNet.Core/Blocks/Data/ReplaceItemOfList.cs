﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Data;

/// <summary>
/// Replaces an item of a list at a specified index
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ReplaceItemOfList : ListExecutionBase
{
    /// <summary>
    /// The provider of the item with that the item at the index should get replaced
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

    /// <summary>
    /// The provider of the index of the item to replace
    /// </summary>
    [Input]
    public IValueProvider IndexProvider
    {
        get => _indexProvider;
        set
        {
            ThrowAtRuntime();
            _indexProvider = value;
        }
    }
    private IValueProvider _indexProvider;

    private const string _constOpCode = "data_replaceitemoflist";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item with that the item at the index should get replaced</param>
    /// <param name="index">The index of the item</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ReplaceItemOfList(ListRef reference, IScratchType item, int index) : this(reference, item, index, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item with that the item at the index should get replaced</param>
    /// <param name="index">The index of the item</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ReplaceItemOfList(ListRef reference, IScratchType item, int index, string blockId) : base(reference, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));

        _itemProvider = item.ConvertToStringValue();
        _indexProvider = new DoubleValue(index);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="itemProvider">The provider of the item with that the item at the index should get replaced</param>
    /// <param name="indexProvider">The provider of the index of the item to replace</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ReplaceItemOfList(ListRef reference, IValueProvider itemProvider, IValueProvider indexProvider) : this(reference, itemProvider, indexProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="itemProvider">The provider of the item with that the item at the index should get replaced</param>
    /// <param name="indexProvider">The provider of the index of the item to replace</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ReplaceItemOfList(ListRef reference, IValueProvider itemProvider, IValueProvider indexProvider, string blockId) : base(reference, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(itemProvider, nameof(itemProvider));
        ArgumentNullException.ThrowIfNull(indexProvider, nameof(indexProvider));

        _itemProvider = itemProvider;
        _indexProvider = indexProvider;
    }

#pragma warning disable CS8618
    internal ReplaceItemOfList(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default)
    {
        double rawIndex = (await IndexProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        int index = (int)Math.Round(rawIndex);

        if (index < 1 || index > list.Values.Count)
        {
            logger.LogTrace("Index to replace item of list is out of range. Index: {index}; Value count: {count}", index, list.Values.Count);
            return;
        }

        IScratchType item = await ItemProvider.GetResultAsync(context, logger, ct);
        list.Values[--index] = item;
    }

    private string GetDebuggerDisplay()
    {
        double rawIndex = IndexProvider.GetDefaultResult().ConvertToDoubleValue();
        int index = (int)Math.Round(rawIndex);

        string itemString = ItemProvider.GetDefaultResult().ConvertToStringValue();

        return string.Format("List {0}[{1}] = \"{2}\"", ListRef.ListName, index, itemString);
    }
}
