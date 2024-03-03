using Newtonsoft.Json.Linq;

namespace ScratchDotNet.Core.StageObjects.Assets;

public class CostumeAsset : AssetBase
{
    /// <summary>
    /// The bitmap resolution
    /// </summary>
    public int BitmapResolution { get; set; }

    /// <summary>
    /// The rotation from center in the X axis
    /// </summary>
    public double RotationCenterX { get; set; }

    /// <summary>
    /// The rotation from center in the Y axis
    /// </summary>
    public double RotationCenterY { get; set; }

    public CostumeAsset(JToken assetToken, string tmpPath) : base(assetToken, tmpPath)
    {
        BitmapResolution = assetToken["bitmapResolution"]!.Value<int>();
        RotationCenterX = assetToken["rotationCenterX"]!.Value<int>();
        RotationCenterY = assetToken["rotationCenterY"]!.Value<int>();
    }
}
