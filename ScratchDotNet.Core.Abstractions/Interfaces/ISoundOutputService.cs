﻿
namespace ScratchDotNet.Core.Services.Interfaces;

/// <summary>
/// An interface that provides the output of sounds with specified parameters
/// </summary>
public interface ISoundOutputService
{
    /// <summary>
    /// Plays the sound that is contained in <paramref name="soundAsset"/>
    /// </summary>
    /// <param name="audioStream">A stream to the audio to play.</param>
    /// <param name="volume">The volume of the sound. The value of this parameter will be between 0.0f (0%) and 1.0f (100%)</param>
    /// <param name="pitch">A modifier for the frequency of the sound. 0.0f is the orginal sound</param>
    /// <param name="panorama">A modifier for the sound that specifies the positioning in a stereo-panarama. The value of this parameter will be between -1.0f (left) and 1.0f (right)</param>
    /// <param name="ct">The cancellation token that cancel the sound play</param>
    /// <returns>The task that awaits the end of playing</returns>
    public Task PlaySoundAsync(Stream audioStream, float volume, float pitch, float panorama, CancellationToken ct = default);
}
