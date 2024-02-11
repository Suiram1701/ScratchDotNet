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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Data;

/// <summary>
/// Removes a item at a specified index
/// </summary>
/// <remarks>
/// The index is 1 based
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class RemoveOfList : ListExecutionBase
{
    /// <summary>
    /// The provider of the index to remove the item from
    /// </summary>
    public IValueProvider IndexProvider { get; }

    private const string _constOpCode = "data_deleteoflist";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="index">The 1 based index of the item to remove</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RemoveOfList(ListRef reference, int index) : this(reference, index, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="index">The 1 based index of the item to remove</param>
    /// <param name="blockId">The id of this blocks</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public RemoveOfList(ListRef reference, int index, string blockId) : base(reference, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));
        IndexProvider = new Result(new DoubleValue(index), DataType.Integer);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="indexProvider">The 1 based index of the item to remove</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RemoveOfList(ListRef reference, IValueProvider indexProvider) : this(reference, indexProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="indexProvider">The 1 based index of the item to remove</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public RemoveOfList(ListRef reference, IValueProvider indexProvider, string blockId) : base(reference, _constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));

        IndexProvider = indexProvider;
        if (IndexProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.Integer;
    }

    internal RemoveOfList(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        IndexProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.INDEX") ?? new Empty(DataType.Integer);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default)
    {
        double rawIndex = (await IndexProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        int index = (int)Math.Round(rawIndex);

        if (index >= 1 && index <= list.Values.Count)
            list.Values.RemoveAt(++index);
        else
            logger.LogTrace("Index to remove item of list is out of range. Index: {index}; Value count: {count}", index, list.Values.Count);
    }

    private string GetDebuggerDisplay()
    {
        double rawIndex = IndexProvider.GetDefaultResult().ConvertToDoubleValue();
        int index = Math.Max((int)Math.Round(rawIndex), 1);

        return string.Format("List {0}.RemoveAt({1})", ListRef.ListName, index);
    }
}
