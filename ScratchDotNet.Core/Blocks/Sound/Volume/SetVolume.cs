using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Sound.Volume;

/// <summary>
/// Set the sound volume of the executor to specified count of percent
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class SetVolume : ExecutionBlockBase
{
    /// <summary>
    /// Provides the count of percent to set
    /// </summary>
    [Input("VOLUME")]
    public IValueProvider VolumeProvider { get; }

    private const string _constOpCode = "sound_setvolumeto";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="volume">The count of percent to set</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public SetVolume(double volume) : this(volume, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="volume">The count of percent to set</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public SetVolume(double volume, string blockId) : base(_constOpCode, blockId)
    {
        if (volume < 0d || volume > 100d)
            throw new ArgumentOutOfRangeException(nameof(volume), volume, "The volume have to be between 0 - 100");
        VolumeProvider = new DoubleValue(volume);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="volumeProvider">The provider of the count of percent to set</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVolume(IValueProvider volumeProvider) : this(volumeProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="volumeProvider">The provider of the count of percent to set</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVolume(IValueProvider volumeProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(volumeProvider, nameof(volumeProvider));
        VolumeProvider = volumeProvider;
    }

#pragma warning disable CS8618
    internal SetVolume(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected internal override async Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double volume = (await VolumeProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        volume = Math.Min(Math.Max(volume, 0d), 100d);     // Validate that the volume is between 0 and 100

        context.Executor.SetSoundVolume(volume);
    }

    private string GetDebuggerDisplay()
    {
        double volume = VolumeProvider.GetDefaultResult().ConvertToDoubleValue();
        return string.Format("set volume to {0}", volume);
    }
}
