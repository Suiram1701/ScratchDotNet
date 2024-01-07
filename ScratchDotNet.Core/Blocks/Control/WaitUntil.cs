using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks.Attributes;
using Scratch.Core.Blocks.Bases;
using Scratch.Core.Blocks.Interfaces;
using System.Diagnostics;

namespace Scratch.Core.Blocks.Control;

/// <summary>
/// Waits until <see cref="ConditionProvider"/> returns <see langword="true"/>
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class WaitUntil : ExecutionBlockBase
{
    /// <summary>
    /// The provider of the condition to wait for
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> means that the condition is empyt
    /// </remarks>
    public IValueProvider? ConditionProvider { get; }

    private const string _constOpCode = "control_wait_until";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="conditionProvider">The provider of the condition to wait for. <see langword="null"/> means that the condition is empty</param>
    /// <param name="blockId">The id of the this block</param>
    public WaitUntil(IValueProvider? conditionProvider, string blockId) : base(_constOpCode, blockId)
    {
        ConditionProvider = conditionProvider;
    }

    internal WaitUntil(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ConditionProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.CONDITION");
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (ConditionProvider is null)     // No condition
            return;

        TaskCompletionSource<bool> tcs = new(TaskCreationOptions.LongRunning);
        using CancellationTokenRegistration ctr = ct.Register(tcs.SetCanceled);

        async void ConditionChangedAsync()
        {
            if ((await ConditionProvider.GetResultAsync(context, logger, ct)).GetBoolValue())
                tcs.SetResult(true);
        }

        try
        {
            ConditionProvider.OnValueChanged += ConditionChangedAsync;

            if ((await ConditionProvider.GetResultAsync(context, logger, ct)).GetBoolValue())
                return;
            await tcs.Task;
        }
        finally
        {
            ConditionProvider.OnValueChanged -= ConditionChangedAsync;
            ctr.Unregister();
        }
    }

    private static string GetDebuggerDisplay()
    {
        return "Wait until";
    }
}
