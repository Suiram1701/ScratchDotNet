using Newtonsoft.Json.Linq;

namespace ScratchDotNet.Core.StageObjects.Assets;

/// <summary>
/// A sound
/// </summary>
public class SoundAsset : AssetBase
{
    /// <summary>
    /// The frequency of the audio
    /// </summary>
    public uint Rate { get; set; }

    /// <summary>
    /// The count of the audio samples
    /// </summary>
    public uint SampleCount { get; set; }

    /// <summary>
    /// Parses a new instance from JSON
    /// </summary>
    /// <param name="assetToken">The JSON token to parse</param>
    public SoundAsset(JToken assetToken, string tmpPath) : base(assetToken, tmpPath)
    {
        Rate = assetToken["rate"]!.Value<uint>();
        SampleCount = assetToken["sampleCount"]!.Value<uint>();
    }

    /// <summary>
    /// Creates a new instance from raw data
    /// </summary>
    /// <param name="name">The id of the asset</param>
    /// <param name="assetId">The id of the asset</param>
    /// <param name="dataFormat">The format of the asset</param>
    /// <param name="rate">The rate of the sound in Hz</param>
    /// <param name="sampleCount">The sample count of the sound</param>
    public SoundAsset(string name, string assetId, string dataFormat, uint rate, uint sampleCount, string tmpPath) : base(name, dataFormat, assetId, tmpPath)
    {
        Rate = rate;
        SampleCount = sampleCount;
    }
}
