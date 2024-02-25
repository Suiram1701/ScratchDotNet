using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Execution;

namespace ScratchDotNet.Core.Blocks.Bases;

/// <summary>
/// The base for every executable block
/// </summary>
public abstract class ExecutionBlockBase : BlockBase
{
    /// <inheritdoc/>
    protected ExecutionBlockBase(string opcode) : base(opcode)
    {
    }

    /// <inheritdoc/>
    protected ExecutionBlockBase(string opCode, string blockId) : base(opCode, blockId)
    {
    }

    /// <inheritdoc/>
    protected internal ExecutionBlockBase(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    /// <summary>
    /// The internally execution of the block
    /// </summary>
    /// <param name="context">The context of the execution</param>
    /// <param name="logger">The logger to use</param>
    /// <param name="ct">The cancellation token</param>
    protected internal abstract Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default);
}