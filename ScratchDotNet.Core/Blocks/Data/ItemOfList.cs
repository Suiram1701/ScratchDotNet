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
/// Provides the item that is contained at a specified index
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ItemOfList : ListOperatorBase
{
    /// <summary>
    /// The provider of the index of the item to get
    /// </summary>
    [Input]
    public IValueProvider IndexProvider { get; }

    private const string _constOpCode = "data_itemoflist";

    /// <summary>
    /// Creates new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="index">The index of the item to get</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ItemOfList(ListRef reference, int index) : this(reference, index, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="index">The index of the item to get</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ItemOfList(ListRef reference, int index, string blockId) : base(reference, blockId, _constOpCode)
    {
        IndexProvider = new DoubleValue(index);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="indexProvider">The provider of the index of the item to get</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ItemOfList(ListRef reference, IValueProvider indexProvider) : this(reference, indexProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="indexProvider">The provider of the index of the item to get</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ItemOfList(ListRef reference, IValueProvider indexProvider, string blockId) : base(reference, blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(indexProvider, nameof(indexProvider));
        IndexProvider = indexProvider;
    }

#pragma warning disable CS8618
    internal ItemOfList(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default)
    {
        double rawIndex = (await IndexProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        int index = (int)Math.Round(rawIndex);

        if (index < 1 || index > list.Values.Count)
        {
            logger.LogTrace("Index to get an item of the list is out of range. Index: {index}; Value count: {count}", index, list.Values.Count);
            return new StringValue();
        }

        return list.Values[--index];
    }

    private string GetDebuggerDisplay()
    {
        double rawIndex = IndexProvider.GetDefaultResult().ConvertToDoubleValue();
        int index = (int)Math.Round(rawIndex);

        return string.Format("List {0}[{1}]", ListRef.ListName, index);
    }
}
