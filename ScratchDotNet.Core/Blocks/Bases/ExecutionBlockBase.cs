using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Scratch.Core.Blocks.Bases;

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
    protected ExecutionBlockBase(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    /// <summary>
    /// Executes this block
    /// </summary>
    /// <param name="context">The context of this execution</param>
    /// <param name="logger">The logger to use</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The task to await</returns>
    public async Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        logger.LogInformation("Start execution of block: {block}; op code: {code}", BlockId, _opCode);

        try
        {
            await ExecuteInternalAsync(context, logger, ct);
        }
        catch (TaskCanceledException) { throw; }
        catch (Exception ex) { logger.LogError(ex, "An error happend while executing substack block {block}", BlockId); }

        logger.LogInformation("Block successfully executed");
    }

    /// <summary>
    /// The internally execution of the block
    /// </summary>
    /// <param name="context">The context of the execution</param>
    /// <param name="logger">The logger to use</param>
    /// <param name="ct">The cancellation token</param>
    protected abstract Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default);
}