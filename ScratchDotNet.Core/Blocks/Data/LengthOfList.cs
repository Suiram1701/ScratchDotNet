using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Data;

/// <summary>
/// Provides the length of a list
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class LengthOfList : ListOperatorBase
{
    private const string _constOpCode = "data_lengthoflist";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <exception cref="ArgumentNullException"></exception>
    public LengthOfList(ListRef reference) : this(reference, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public LengthOfList(ListRef reference, string blockId) : base(reference, blockId, _constOpCode)
    {
    }

    internal LengthOfList(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    protected override Task<IScratchType> GetResultAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default) =>
        Task.FromResult<IScratchType>(new DoubleValue(list.Values.Count));

    private string GetDebuggerDisplay() =>
        string.Format("List {0}.Count", ListRef.ListName);
}
