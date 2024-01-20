using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.StageObjects.Assets;

namespace ScratchDotNet.Core.StageObjects;

/// <summary>
/// Provides all properties a object on the stage needs
/// </summary>
public interface IStageObject
{
    /// <summary>
    /// The name of the figure
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The variables owned by this object
    /// </summary>
    public IEnumerable<Variable> Variables { get; }

    // Lists

    // Broadcasts

    /// <summary>
    /// The id of the current costume
    /// </summary>
    public int CurrentCostum { get; }

    /// <summary>
    /// The costumes owned by this object
    /// </summary>
    public IEnumerable<CostumeAsset> Costumes { get; }

    /// <summary>
    /// The sounds owned by this object
    /// </summary>
    public IEnumerable<SoundAsset> Sounds { get; }

    /// <summary>
    /// The sound volume of this object in percent
    /// </summary>
    public int SoundVolume { get; }

    /// <summary>
    /// A modifier for the sound frequency
    /// </summary>
    internal double SoundPitch { get; }

    /// <summary>
    /// A modifier for the sound that specifies the positioning in a stereo-panarama
    /// </summary>
    internal double SoundPanorama { get; }

    /// <summary>
    /// A cancellation token source for sound playing
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> when currently no sound is playing
    /// </remarks>
    internal CancellationTokenSource? SoundCts { get; set; }

    /// <summary>
    /// The graphical order of this object
    /// </summary>
    public int LayerOrder { get; }
}
