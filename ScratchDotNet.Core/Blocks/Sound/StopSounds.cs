using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Execution;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Sound;

/// <summary>
/// Stops the current playing sound of the executor
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class StopSounds : ExecutionBlockBase
{
    private const string _constOpCode = "sound_stopallsounds";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public StopSounds() : base(_constOpCode)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The id of this block</param>
    public StopSounds(string blockId) : base(_constOpCode, blockId)
    {
    }

    internal StopSounds(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    protected internal override Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        context.RuntimeData.TryGetValue($"{context.Executor.Name}_soundCts", out object? soundCtsObj);
        (soundCtsObj as CancellationTokenSource)?.Cancel();

        return Task.CompletedTask;
    }

    private static string GetDebuggerDisplay() =>
        "Stop sounds";
}
