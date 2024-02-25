using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Execution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Control;

/// <summary>
/// Executes the <see cref="Substack"/> forever until the execution will be cancelled
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Forever : ExecutionBlockBase
{
    /// <summary>
    /// The substack to execute forever
    /// </summary>
    [Substack]
    public Substack Substack { get; }

    private const string _constOpCode = "control_forever";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="substack">The substack to execute forever</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Forever(IList<ExecutionBlockBase> substack) : this(substack, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="substack">The substack to execute forever</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Forever(IList<ExecutionBlockBase> substack, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(substack, nameof(substack));
        Substack = new(substack);
    }

#pragma warning disable CS8618
    internal Forever(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
            await Substack.InvokeAsync(context, logger, ct);
    }

    private string GetDebuggerDisplay() =>
        string.Format("Forever: Substack: {0} blocks", Substack.Count);
}
