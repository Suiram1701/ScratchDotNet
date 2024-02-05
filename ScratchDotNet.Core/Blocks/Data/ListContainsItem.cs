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
/// Determines whether a list contains a specified item
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ListContainsItem : ListOperatorBase, IBoolValueProvider
{
    /// <summary>
    /// The provider of the item which one could be contained in the list
    /// </summary>
    public IValueProvider ItemProvider { get; }

    private const string _constOpCode = "data_listcontainsitem";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item which one could be contained in the list</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ListContainsItem(ListRef reference, ScratchTypeBase item) : this(reference, item, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item which one could be contained in the list</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref=ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ListContainsItem(ListRef reference, ScratchTypeBase item, string blockId) : base(reference, blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));
        ItemProvider = new Result(item, DataType.String);
    }

    internal ListContainsItem(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ItemProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.ITEM") ?? new Empty(DataType.String);
    }

    protected override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default)
    {
        ScratchTypeBase item = await ItemProvider.GetResultAsync(context, logger, ct);

        bool result = list.Values.Contains(item);
        return new BooleanType(result);
    }

    private string GetDebuggerDisplay()
    {
        string itemString = ItemProvider.GetDefaultResult().GetStringValue();

        return string.Format("List {0}.Conatins(\"{1}\")", ListRef.ListName, itemString);
    }
}
