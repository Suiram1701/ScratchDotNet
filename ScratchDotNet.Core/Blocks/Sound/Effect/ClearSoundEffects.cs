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

namespace ScratchDotNet.Core.Blocks.Sound.Effect;

/// <summary>
/// Clears all sound effects
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ClearSoundEffects : ExecutionBlockBase
{
    private const string _constOpCode = "sound_cleareffects";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public ClearSoundEffects() : this(BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The id of this block</param>
    public ClearSoundEffects(string blockId) : base(_constOpCode, blockId)
    {
    }

    internal ClearSoundEffects(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    protected override Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        context.Executor.SoundPitch = 0f;
        context.Executor.SoundPanorama = 0f;

        return Task.CompletedTask;
    }

    private static string GetDebuggerDisplay() =>
        "Clear all sound effects";
}
