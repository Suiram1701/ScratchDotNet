using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public IValueProvider ItemProvider { get; }

    /// <summary>
    /// The provider of the index where the item should be insert
    /// </summary>
    public IValueProvider IndexProvider { get; }

    private const string _constOpCode = "data_insertatlist";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item to insert at the index</param>
    /// <param name="index">The 1 based index where the item should be insert to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public InsertInList(ListRef reference, IScratchType item, int index) : this(reference, item, index, BlockHelpers.GenerateBlockId())
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
    public InsertInList(ListRef reference, IScratchType item, int index, string blockId) : base(reference, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));

        ItemProvider = item.ConvertToStringValue();
        IndexProvider = new DoubleValue(index);
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

        ItemProvider = itemProvider;
        IndexProvider = indexProvider;
    }

    internal InsertInList(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ItemProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.ITEM");
        IndexProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.INDEX");
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
