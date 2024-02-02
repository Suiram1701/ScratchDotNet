using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.Execution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Data;

/// <summary>
/// Removes all items out of a list
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class RemoveAllOfList : ListExecutionBase
{
    private const string _constOpCode = "data_deletealloflist";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list to removes the items from</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RemoveAllOfList(ListRef reference) : base(reference, _constOpCode)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The reference to the list to removes the items from</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public RemoveAllOfList(ListRef reference, string blockId) : base(reference, _constOpCode, blockId)
    {
    }

    internal RemoveAllOfList(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    protected override Task ExecuteInternalAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default)
    {
        list.Values.Clear();
        return Task.CompletedTask;
    }

    private string GetDebuggerDisplay() =>
        string.Format("List {0}.Clear()", ListRef.ListName);
}
