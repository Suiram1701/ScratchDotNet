using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Control;

/// <summary>
/// Stops the execution of scripts depended on the stop scope
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class StopScript : ExecutionBlockBase
{
    /// <summary>
    /// The scope of the blocks that will be stopped
    /// </summary>
    public StopScope Scope { get; }

    private const string _constOpCode = "control_stop";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="scope">The scope of the scripts that will be stopped</param>
    public StopScript(StopScope scope) : base(_constOpCode)
    {
        ArgumentNullException.ThrowIfNull(scope, nameof(scope));
        Scope = scope;
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="scope">The scope of the scripts that will be stopped</param>
    /// <param name="blockId">The id of this block</param>
    public StopScript(StopScope scope, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(scope, nameof(scope));
        Scope = scope;
    }

    internal StopScript(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        string stopScopeString = blockToken.SelectToken("fields.STOP_OPTION[0]")!.Value<string>()!;
        Scope = EnumNameAttributeHelpers.ParseEnumWithName<StopScope>(stopScopeString);
    }

    protected override Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        switch (Scope)
        {
            case StopScope.This:
                context.Executor.ScriptsCts
                    .First(cts => cts.Token.Equals(ct))
                    .Cancel();
                break;
            case StopScope.All:
                foreach (CancellationTokenSource cts in context.Stage.ScriptsCts)
                    cts.Cancel();

                foreach (IStageObject stageObj in context.Figures)
                {
                    foreach (CancellationTokenSource cts in stageObj.ScriptsCts)
                        cts.Cancel();
                }
                break;
            case StopScope.OtherScripts:
                foreach (CancellationTokenSource cts in context.Executor.ScriptsCts)
                {
                    if (cts.Token.Equals(ct))
                        continue;

                    cts.Cancel();
                }
                break;
        }

        return Task.CompletedTask;
    }

    private string GetDebuggerDisplay()
    {
        string stopScopeString = EnumNameAttributeHelpers.GetNames<StopScope>()
            .First(kv => kv.Value == Scope).Key;

        return string.Format("Stop: {0}", stopScopeString);
    }
}
