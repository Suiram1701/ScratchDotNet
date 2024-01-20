using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Providers.Interfaces;
using ScratchDotNet.Core.StageObjects;
using ScratchDotNet.Core.StageObjects.Assets;
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
    public IValueProvider SoundNameProvider { get; }

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
        SoundNameProvider = new SoundNameReporter(sound);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// Providers that implements <see cref="IConstProvider"/> are not supported for this block. To provide a constant sound name you have to use a constructor that takes a instance of <see cref="SoundAsset"/> to refer to the sound to use.
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
    /// Providers that implements <see cref="IConstProvider"/> are not supported for this block. To provide a constant sound name you have to use a constructor that takes a instance of <see cref="SoundAsset"/> to refer to the sound to use.
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

        if (soundNameProvider is IConstProvider)
        {
            string message = string.Format(
                "Providers that implements {0} are not supported for this block. To provide a constant sound name you have to use a constructor that takes a instance of {1} to refer to the sound to use.",
                nameof(IConstProvider),
                nameof(SoundAsset));
            throw new NotSupportedException(message);
        }

        SoundNameProvider = soundNameProvider;
    }

    internal PlaySound(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        SoundNameProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.SOUND_MENU")
            ?? throw new ArgumentNullException(nameof(blockToken), string.Format("Could not determine the sound name of block {0}", blockId));

        AwaitEnd = _opCode.Equals(_constPlayUntilDoneOpCode);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        IStageObject executor = context.Executor;
        if (!(executor.SoundCts?.IsCancellationRequested ?? true))
            executor.SoundCts.Cancel();

        if (context.Services[typeof(ISoundOutputService)] is not ISoundOutputService soundOutputProvider)
        {
            logger.LogCritical("Could not find any registered service that implements {provider}", nameof(ISoundOutputService));
            return;
        }

        string soundName = (await SoundNameProvider.GetResultAsync(context, logger, ct)).GetStringValue();
        SoundAsset? soundAsset = executor.Sounds.FirstOrDefault(sa => sa.Name.Equals(soundName));
        if (soundAsset is null)
        {
            logger.LogWarning("Could not find a sound named \"{name}\"", soundName);
            return;
        }

        float volume = Math.Min(Math.Max(executor.SoundVolume, 0), 100) / 100f;      // Validate that the volume is between 0 and 100 and convert it to a value between 0.0 - 1.0
        float pitch = (float)executor.SoundPitch;
        float pan = (float)Math.Min(Math.Max(executor.SoundPanorama, -100), 100) / 100;     // Validate that the pan is between 0 and 100 and convert it to a value between 0.0 - 1.0

        executor.SoundCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        Task soundPlay = soundOutputProvider.PlaySoundAsync(soundAsset, volume, pitch, pan, logger, executor.SoundCts.Token);
        if (AwaitEnd)
            await soundPlay;
        else
            _ = Task.Run(async () => await soundPlay, executor.SoundCts.Token);
    }

    private static string GetOpCodeFromAwaitEnd(bool awaitEnd) =>
        awaitEnd
        ? _constPlayUntilDoneOpCode
        : _constPlayUntilDoneOpCode;

    private string GetDebuggerDisplay()
    {
        string name = SoundNameProvider.GetDefaultResult().GetStringValue();
        string untilDone = AwaitEnd
            ? " until end"
            : string.Empty;
        return string.Format("Play sound{0}: {1}", untilDone, name);
    }
}
