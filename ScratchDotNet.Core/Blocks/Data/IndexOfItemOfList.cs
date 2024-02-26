using Microsoft.Extensions.Logging;
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
/// Provides the 1 based index of an item in the list
/// </summary>
/// <remarks>
/// If more than one item was found that equals the specified item the first index will be returned
/// </remarks>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal class IndexOfItemOfList : ListOperatorBase
{
    /// <summary>
    /// The item whose index should get read
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

    private const string _constOpCode = "data_itemnumoflist";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item whose index should get returned</param>
    /// <exception cref="ArgumentNullException"></exception>
    public IndexOfItemOfList(ListRef reference, string item) : this(reference, item, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="item">The item whose index should get returned</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public IndexOfItemOfList(ListRef reference, string item, string blockId) : base(reference, blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        _itemProvider = new StringValue(item);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="itemProvider">The provider of the item whose index should get returned</param>
    /// <exception cref="ArgumentNullException"></exception>
    public IndexOfItemOfList(ListRef reference, IValueProvider itemProvider) : this(reference, itemProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="itemProvider">The provider of the item whose index should get returned</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public IndexOfItemOfList(ListRef reference, IValueProvider itemProvider, string blockId) : base(reference, blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(itemProvider, nameof(itemProvider));
        _itemProvider = itemProvider;
    }

#pragma warning disable CS8618
    internal IndexOfItemOfList(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default)
    {
        IScratchType item = await ItemProvider.GetResultAsync(context, logger, ct);

        int index = list.Values.IndexOf(item);
        return new DoubleValue(index + 1);
    }

    private string GetDebuggerDisplay()
    {
        string itemString = ItemProvider.GetDefaultResult().ConvertToStringValue();

        return string.Format("List {0}.IndexOf(\"{1}\");", ListRef.ListName, itemString);
    }
}
