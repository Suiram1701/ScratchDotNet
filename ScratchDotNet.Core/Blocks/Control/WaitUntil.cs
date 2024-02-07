using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Execution;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Control;

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
    public IBoolValueProvider ConditionProvider { get; }

    private const string _constOpCode = "control_wait_until";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public WaitUntil() : base(_constOpCode)
    {
        ConditionProvider = new EmptyBool();
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="conditionProvider">The provider of the condition to wait for</param>
    public WaitUntil(IBoolValueProvider conditionProvider) : base(_constOpCode)
    {
        ConditionProvider = conditionProvider;
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="conditionProvider">The provider of the condition to wait for</param>
    /// <param name="blockId">The id of the this block</param>
    public WaitUntil(IBoolValueProvider conditionProvider, string blockId) : base(_constOpCode, blockId)
    {
        ConditionProvider = conditionProvider;
    }

    internal WaitUntil(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ConditionProvider = BlockHelpers.GetBoolDataProvider(blockToken, "inputs.CONDITION");
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (ConditionProvider is EmptyBool)     // No condition
            return;

        TaskCompletionSource tcs = new(TaskCreationOptions.LongRunning);
        using CancellationTokenRegistration ctr = ct.Register(tcs.SetCanceled);

        // This method is called when the result of a value provider on which the condition depends may have changed
        async void ConditionChangedAsync(object? s, System.EventArgs e)
        {
            if ((await ConditionProvider.GetResultAsync(context, logger, ct)).GetBoolValue())
                tcs.SetResult();
        }

        try
        {
            if ((await ConditionProvider.GetResultAsync(context, logger, ct)).GetBoolValue())
                return;
            ConditionProvider.OnValueChanged += ConditionChangedAsync;

            await tcs.Task;     // Wait until the condition were fulfilled
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error happened while waiting for the {tcs}", nameof(TaskCompletionSource));
        }
        finally     // The condition were fulfilled or the execution was cancelled
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
