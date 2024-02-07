using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Sound.Volume;

/// <summary>
/// Provides the volume of the executor in percent
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class VolumeValue : ValueOperatorBase
{
    public override event EventHandler<ValueChangedEventArgs>? OnValueChanged;

    private bool _delegateInitialized;

    private const string _constOpCode = "sound_volume";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public VolumeValue() : base(_constOpCode)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The id of this block</param>
    public VolumeValue(string blockId) : base(blockId, _constOpCode)
    {
    }

    internal VolumeValue(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    public override Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (!_delegateInitialized)
        {
            context.Executor.OnVolumeChanged += Executor_OnVolumeChanged;

            logger.LogInformation("Value changed event of block {block} was successfully initialized", BlockId);
            _delegateInitialized = true;
        }

        double result = context.Executor.SoundVolume;
        return Task.FromResult((ScratchTypeBase)new NumberType(result));
    }

    private void Executor_OnVolumeChanged(object? sender, GenericValueChangedEventArgs<double> e)
    {
        if (e.OldValue == e.NewValue)
            return;

        ValueChangedEventArgs eventArgs = new(new NumberType(e.OldValue), new  NumberType(e.NewValue));
        OnValueChanged?.Invoke(sender, eventArgs);
    }

    private static string GetDebuggerDisplay() =>
        "get volume";
}
