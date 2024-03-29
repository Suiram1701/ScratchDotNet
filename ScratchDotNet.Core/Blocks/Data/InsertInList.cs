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
/// Inserts a item into a specified index into a list
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class InsertInList : ListExecutionBase
{
    /// <summary>
    /// The provider of the item to insert
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
    /// The provider of the index where the item should be insert
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

    private const string _constOpCode = "data_insertatlist";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item to insert at the index</param>
    /// <param name="index">The 1 based index where the item should be insert to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public InsertInList(ListRef reference, string item, int index) : this(reference, item, index, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item to insert at the index</param>
    /// <param name="index">The 1 based index where the item should be insert to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public InsertInList(ListRef reference, string item, int index, string blockId) : base(reference, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));

        _itemProvider = new StringValue(item);
        _indexProvider = new DoubleValue(index);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="itemProvider">The provider of the item to insert at the index</param>
    /// <param name="indexProvider">The provider of the 1 based index where the item should be insert to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public InsertInList(ListRef reference, IValueProvider itemProvider, IValueProvider indexProvider) : this(reference, itemProvider, indexProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="itemProvider">The provider of the item to insert at the index</param>
    /// <param name="indexProvider">The provider of the 1 based index where the item should be insert to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public InsertInList(ListRef reference, IValueProvider itemProvider, IValueProvider indexProvider, string blockId) : base(reference, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(itemProvider, nameof(itemProvider));
        ArgumentNullException.ThrowIfNull(indexProvider, nameof(indexProvider));

        _itemProvider = itemProvider;
        _indexProvider = indexProvider;
    }

#pragma warning disable CS8618
    internal InsertInList(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default)
    {
        double rawIndex = (await IndexProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        int index = (int)Math.Round(rawIndex);

        if (index < 1 || index > (list.Values.Count + 1))
        {
            logger.LogTrace("Index to insert item into list is out of range. Index: {index}; Value count: {count}", index, list.Values.Count + 1);
            return;
        }

        IScratchType item = await ItemProvider.GetResultAsync(context, logger, ct);
        list.Values.Insert(--index, item);
    }

    private string GetDebuggerDisplay()
    {
        string itemString = ItemProvider.GetDefaultResult().ConvertToStringValue();

        double rawIndex = IndexProvider.GetDefaultResult().ConvertToDoubleValue();
        int index = (int)Math.Round(rawIndex);

        return string.Format("List {0}.Insert({1}, \"{2}\")", ListRef.ListName, index, itemString);
    }
}
