using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.StageObjects.Assets;

namespace ScratchDotNet.Core.Providers.Interfaces;

/// <summary>
/// An interface that provides the output of sounds with specified parameters
/// </summary>
public interface ISoundOutputService
{
    /// <summary>
    /// Plays the sound that is contained in <paramref name="soundAsset"/>
    /// </summary>
    /// <param name="soundAsset">The sound to play</param>
    /// <param name="volume">The volume of the sound. The value of this parameter will be between 0.0f (0%) and 1.0f (100%)</param>
    /// <param name="pitch">A modifier for the frequency of the sound. 0.0f is the orginal sound</param>
    /// <param name="panorama">A modifier for the sound that specifies the positioning in a stereo-panarama. The value of this parameter will be between -1.0f (left) and 1.0f (right)</param>
    /// <param name="ct">The cancellation token that cancel the sound play</param>
    /// <param name="logger">A logger that could be used to log errors or invalid data</param>
    /// <returns>The task that awaits the end of playing</returns>
    public Task PlaySoundAsync(SoundAsset soundAsset, float volume, float pitch, float panorama, ILogger logger, CancellationToken ct = default);
}
