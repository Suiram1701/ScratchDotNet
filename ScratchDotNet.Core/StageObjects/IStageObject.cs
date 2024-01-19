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
    /// The graphical order of this object
    /// </summary>
    public int LayerOrder { get; }

    /// <summary>
    /// The sound volume of this object in percent
    /// </summary>
    public int Volume { get; }
}
