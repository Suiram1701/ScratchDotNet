using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.StageObjects.Assets;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Sound;

/// <summary>
/// The reporter provider for the name of a sound
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal class SoundNameReporter : ValueOperatorBase
{
    /// <inheritdoc/>
    /// <remarks>
    /// This will be never get called
    /// </remarks>
    public override event EventHandler<ValueChangedEventArgs> OnValueChanged { add { } remove { } }

    /// <summary>
    /// The name of the sound
    /// </summary>
    public string Name { get; }

    private const string _constOpCode = "sound_sounds_menu";

    /// <summary>
    /// Creates a new instance that have a random block id
    /// </summary>
    /// <param name="name">The name of the sound the refer to</param>
    /// <exception cref="ArgumentException"></exception>
    public SoundNameReporter(string name) : this(name, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="name">The name of the sound to refer to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    public SoundNameReporter(string name, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

        Name = name;
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="sound">The sound asset to refer to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SoundNameReporter(SoundAsset sound) : this(sound, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="sound">The sound asset to refer to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SoundNameReporter(SoundAsset sound, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(sound, nameof(sound));
        Name = sound.Name;
    }

    internal SoundNameReporter(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        Name = blockToken.SelectToken("fields.SOUND_MENU[0]")!.Value<string>()!;
    }

    public override Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default) =>
        Task.FromResult((IScratchType)new StringValue(Name));

    private string GetDebuggerDisplay() =>
        string.Format("Sound: {0}", Name);
}
