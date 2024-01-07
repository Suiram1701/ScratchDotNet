using Newtonsoft.Json.Linq;

namespace Scratch.Core.Figure.Assets;

public class CostumeAsset : AssetBase
{
    /// <summary>
    /// The bitmap resolution
    /// </summary>
    public int BitmapResolution { get; set; }

    /// <summary>
    /// The rotation from center in the X axis
    /// </summary>
    public int RotationCenterX { get; set; }

    /// <summary>
    /// The rotation from center in the Y axis
    /// </summary>
    public int RotationCenterY { get; set; }

    public CostumeAsset(JToken assetToken, string tmpPath) : base(assetToken, tmpPath)
    {
        BitmapResolution = assetToken["bitmapResolution"]!.Value<int>();
        RotationCenterX = assetToken["rotationCenterX"]!.Value<int>();
        RotationCenterY = assetToken["rotationCenterY"]!.Value<int>();
    }
}
