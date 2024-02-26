using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Services.Interfaces;
using ScratchDotNet.Core.StageObjects.Assets;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Sound;

/// <summary>
/// Plays a sound that is owned by the executor of this block
/// </summary>
[ExecutionBlockCode(_constPlayOpCode)]
[ExecutionBlockCode(_constPlayUntilDoneOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class PlaySound : ExecutionBlockBase
{
    /// <summary>
    /// The provider of the name of the sound to play
    /// </summary>
    [Input("SOUND_MENU")]
    public IValueProvider SoundNameProvider
    {
        get => _soundNameProvider;
        set
        {
            ThrowAtRuntime();
            _soundNameProvider = value;
        }
    }
    private IValueProvider _soundNameProvider;

    /// <summary>
    /// Indicates whether the end of playing the sound should be awaited
    /// </summary>
    public bool AwaitEnd { get; }

    private const string _constPlayOpCode = "sound_play";
    private const string _constPlayUntilDoneOpCode = "sound_playuntildone";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="sound">The sound asset that should be played</param>
    /// <param name="awaitEnd">Indicates the end of the playing the sound should be awaited</param>
    /// <exception cref="ArgumentNullException"></exception>
    public PlaySound(SoundAsset sound, bool awaitEnd) : this(sound, awaitEnd, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="sound">The sound asset that should be played</param>
    /// <param name="awaitEnd">Indicates the end of the playing the sound should be awaited</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public PlaySound(SoundAsset sound, bool awaitEnd, string blockId) : base(GetOpCodeFromAwaitEnd(awaitEnd), blockId)
    {
        ArgumentNullException.ThrowIfNull(sound, nameof(sound));
        _soundNameProvider = new SoundNameReporter(sound);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// A providers that implements <see cref="IScratchType"/> are not supported for this block. To provide a constant sound name you have to use a constructor that takes a instance of <see cref="SoundAsset"/> to refer to the sound to use.
    /// </remarks>
    /// <param name="soundNameProvider">The provider of the sound name</param>
    /// <param name="awaitEnd">Indicates the end of the playing the sound should be awaited</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public PlaySound(IValueProvider soundNameProvider, bool awaitEnd) : this(soundNameProvider, awaitEnd, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// A providers that implements <see cref="IScratchType"/> are not supported for this block. To provide a constant sound name you have to use a constructor that takes a instance of <see cref="SoundAsset"/> to refer to the sound to use.
    /// </remarks>
    /// <param name="soundNameProvider">The provider of the sound name</param>
    /// <param name="awaitEnd">Indicates the end of the playing the sound should be awaited</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public PlaySound(IValueProvider soundNameProvider, bool awaitEnd, string blockId) : base(GetOpCodeFromAwaitEnd(awaitEnd), blockId)
    {
        ArgumentNullException.ThrowIfNull(soundNameProvider, nameof(soundNameProvider));

        if (soundNameProvider is IScratchType)
        {
            string message = string.Format(
                "Providers that implements {0} are not supported for this block. To provide a constant sound name you have to use a constructor that takes a instance of {1} to refer to the sound to use.",
                nameof(IScratchType),
                nameof(SoundAsset));
            throw new NotSupportedException(message);
        }

        _soundNameProvider = soundNameProvider;
    }

#pragma warning disable CS8618
    internal PlaySound(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
        AwaitEnd = _opCode.Equals(_constPlayUntilDoneOpCode);
    }

    protected internal override async Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        context.RuntimeData.TryGetValue($"{context.Executor.Name}_soundCts", out object? soundCtsObj);
        CancellationTokenSource? soundCts = soundCtsObj as CancellationTokenSource;

        if (!(soundCts?.IsCancellationRequested ?? true))     // Stop sound playing when the object still playing a sound
            soundCts.Cancel();

        if (context.Services[typeof(ISoundOutputService)] is not ISoundOutputService soundOutputService)
        {
            logger.LogCritical("Could not find any registered service that implements {provider}", nameof(ISoundOutputService));
            return;
        }

        string soundName = (await SoundNameProvider.GetResultAsync(context, logger, ct)).ConvertToStringValue();
        SoundAsset? soundAsset = context.Executor.Sounds.FirstOrDefault(sa => sa.Name.Equals(soundName));
        if (soundAsset is null)
        {
            logger.LogTrace("Could not find a sound named \"{name}\"", soundName);
            return;
        }

        context.RuntimeData.TryGetValue($"{context.Executor.Name}_{nameof(SoundEffect.Pitch)}", out object? pitchObj);
        double soundPitch = pitchObj as double? ?? 0.0d;

        context.RuntimeData.TryGetValue($"{context.Executor.Name}_{nameof(SoundEffect.Panorama)}", out object? panObj);
        double soundPan = panObj as double? ?? 0.0d;

        float volume = (float)(Math.Min(Math.Max(context.Executor.SoundVolume, 0), 100) / 100f);      // Validate that the volume is between 0 and 100 and convert it to a value between 0.0 - 1.0
        float pitch = (float)soundPitch;
        float pan = (float)Math.Min(Math.Max(soundPan, -100), 100) / 100;     // Validate that the pan is between 0 and 100 and convert it to a value between 0.0 - 1.0

        CancellationTokenSource newSoundCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        context.RuntimeData[$"{context.Executor.Name}_soundCts"] = newSoundCts;

        Task soundPlay = soundOutputService.PlaySoundAsync(soundAsset, volume, pitch, pan, newSoundCts.Token);
        if (AwaitEnd)
            await soundPlay;
        else
            _ = Task.Run(async () => await soundPlay, newSoundCts.Token);
    }

    private static string GetOpCodeFromAwaitEnd(bool awaitEnd) =>
        awaitEnd
        ? _constPlayUntilDoneOpCode
        : _constPlayUntilDoneOpCode;

    private string GetDebuggerDisplay()
    {
        string name = SoundNameProvider.GetDefaultResult().ConvertToStringValue();
        string untilDone = AwaitEnd
            ? " until end"
            : string.Empty;
        return string.Format("Play sound{0}: {1}", untilDone, name);
    }
}
