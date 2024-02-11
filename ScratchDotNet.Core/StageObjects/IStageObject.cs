using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.StageObjects.Assets;

namespace ScratchDotNet.Core.Types.Interfaces;

/// <summary>
/// Provides all properties a object on the stage needs
/// </summary>
public interface IStageObject
{
    /// <summary>
    /// Called when the volume of the object could have been changed
    /// </summary>
    internal event EventHandler<GenericValueChangedEventArgs<double>> OnVolumeChanged;

    /// <summary>
    /// The name of the figure
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The variables owned by this object
    /// </summary>
    public IEnumerable<Variable> Variables { get; }

    /// <summary>
    /// The lists owned by this object
    /// </summary>
    public IEnumerable<List> Lists { get; }

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
    public double SoundVolume { get; set; }

    /// <summary>
    /// The graphical order of this object
    /// </summary>
    public int LayerOrder { get; }

    /// <summary>
    /// A list of the cts of every script owned by this object
    /// </summary>
    internal IEnumerable<CancellationTokenSource> ScriptsCts { get; }
}
