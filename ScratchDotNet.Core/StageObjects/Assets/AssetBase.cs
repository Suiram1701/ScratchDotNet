using Newtonsoft.Json.Linq;

namespace ScratchDotNet.Core.StageObjects.Assets;

/// <summary>
/// The base for every asset
/// </summary>
public abstract class AssetBase
{
    /// <summary>
    /// The id of the asset
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The file extension of the data
    /// </summary>
    public string DataFormat { get; }

    /// <summary>
    /// The id of the asset
    /// </summary>
    protected readonly string _assetId;

    /// <summary>
    /// The path where the sb3 was extracted
    /// </summary>
    private readonly string _tmpPath;

    /// <summary>
    /// Parses the asset from JSON
    /// </summary>
    /// <param name="assetToken"></param>
    /// <param name="tmpPath">The directory where the asset is saved</param>
    protected AssetBase(JToken assetToken, string tmpPath)
    {
        Name = assetToken["name"]!.Value<string>()!;
        DataFormat = assetToken["dataFormat"]!.Value<string>()!;
        _assetId = assetToken["assetId"]!.Value<string>()!;
        _tmpPath = tmpPath;
    }

    /// <summary>
    /// Set the values
    /// </summary>
    /// <param name="name">The id of the asset</param>
    /// <param name="dataFormat">The file extension of the data</param>
    /// <param name="assetId">The asset id</param>
    /// <param name="tmpPath">The directory where the asset is saved</param>
    protected AssetBase(string name, string dataFormat, string assetId, string tmpPath)
    {
        Name = name;
        DataFormat = dataFormat;
        _assetId = assetId;
        _tmpPath = tmpPath;
    }

    /// <summary>
    /// Gets the resource stream of the asset
    /// </summary>
    /// <returns>The stream</returns>
    public virtual Stream GetStream()
    {
        string assetPath = Path.Combine(_tmpPath, _assetId);
        assetPath = Path.GetFileName(assetPath);
        return new FileStream(assetPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}
