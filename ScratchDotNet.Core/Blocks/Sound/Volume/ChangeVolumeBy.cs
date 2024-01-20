using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Sound.Volume;

/// <summary>
/// Changes the volume of the executor by a specified count of percent
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ChangeVolumeBy : ExecutionBlockBase
{
    /// <summary>
    /// The provider of the count of percent to change the volume by
    /// </summary>
    public IValueProvider ValueProvider { get; }

    private const string _constOpCode = "sound_changevolumeby";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public ChangeVolumeBy() : base(_constOpCode)
    {
        ValueProvider = new Result(0, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The value to change the volume by</param>
    public ChangeVolumeBy(double value) : this(value, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="value">The value to change the volume by</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    public ChangeVolumeBy(double value, string blockId) : base(_constOpCode, blockId)
    {
        ValueProvider = new Result(value, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="valueProvider">The provider of the value to change the volume by</param>
    public ChangeVolumeBy(IValueProvider valueProvider) : this(valueProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="valueProvider">The provider of the value to change the volume by</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    public ChangeVolumeBy(IValueProvider valueProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(valueProvider, nameof(valueProvider));

        ValueProvider = valueProvider;
        if (ValueProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.Number;
    }

    internal ChangeVolumeBy(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        ValueProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.VOLUME") ?? new Empty(DataType.Number);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double value = (await ValueProvider.GetResultAsync(context, logger, ct)).GetNumberValue();
        double volume = context.Executor.SoundVolume + value;

        volume = Math.Min(Math.Max(volume, 0), 100);     // Validate that the volume id between 0 and 100
        context.Executor.SoundVolume = volume;
    }

    private string GetDebuggerDisplay()
    {
        double value = ValueProvider.GetDefaultResult().GetNumberValue();
        return string.Format("change volume by {0}", value);
    }
}
