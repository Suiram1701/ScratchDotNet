using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
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

        TaskCompletionSource<bool> tcs = new(TaskCreationOptions.LongRunning);
        using CancellationTokenRegistration ctr = ct.Register(tcs.SetCanceled);

        async void ConditionChangedAsync()
        {
            if ((await ConditionProvider.GetResultAsync(context, logger, ct)).GetBoolValue())
                tcs.SetResult(true);
        }

        try
        {
            if ((await ConditionProvider.GetResultAsync(context, logger, ct)).GetBoolValue())
                return;
            ConditionProvider.OnValueChanged += ConditionChangedAsync;

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
