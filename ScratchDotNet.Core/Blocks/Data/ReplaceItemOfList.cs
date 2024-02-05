using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public IValueProvider ItemProvider { get; }

    /// <summary>
    /// The provider of the index of the item to replace
    /// </summary>
    public IValueProvider IndexProvider { get; }

    private const string _constOpCode = "data_replaceitemoflist";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item with that the item at the index should get replaced</param>
    /// <param name="index">The index of the item</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ReplaceItemOfList(ListRef reference, ScratchTypeBase item, int index) : this(reference, item, index, BlockHelpers.GenerateBlockId())
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
    public ReplaceItemOfList(ListRef reference, ScratchTypeBase item, int index, string blockId) : base(reference, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));

        ItemProvider = new Result(item, DataType.String);
        IndexProvider = new Result(new NumberType(index), DataType.Integer);
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

        ItemProvider = itemProvider;
        if (ItemProvider is IConstProvider constItemProvider)
            constItemProvider.DataType = DataType.String;

        IndexProvider = indexProvider;
        if (IndexProvider is IConstProvider constIndexProvider)
            constIndexProvider.DataType = DataType.Integer;
    }

    internal ReplaceItemOfList(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ItemProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.ITEM") ?? new Empty(DataType.String);
        IndexProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.INDEX") ?? new Empty(DataType.Integer);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default)
    {
        double rawIndex = (await IndexProvider.GetResultAsync(context, logger, ct)).GetNumberValue();
        int index = (int)Math.Round(rawIndex);

        if (index < 1 || index > list.Values.Count)
        {
            logger.LogTrace("Index to replace item of list is out of range. Index: {index}; Value count: {count}", index, list.Values.Count);
            return;
        }

        ScratchTypeBase item = await ItemProvider.GetResultAsync(context, logger, ct);
        list.Values[--index] = item;
    }

    private string GetDebuggerDisplay()
    {
        double rawIndex = IndexProvider.GetDefaultResult().GetNumberValue();
        int index = (int)Math.Round(rawIndex);

        string itemString = ItemProvider.GetDefaultResult().GetStringValue();

        return string.Format("List {0}[{1}] = \"{2}\"", ListRef.ListName, index, itemString);
    }
}
