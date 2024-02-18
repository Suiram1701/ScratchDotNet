using Microsoft.Extensions.Logging;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ScratchDotNet.Core.NAudio.Wave;
using ScratchDotNet.Core.Services.Interfaces;
using ScratchDotNet.Core.StageObjects.Assets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Services;

/// <summary>
/// A default implementation of <see cref="ISoundOutputService"/> that uses NAudio
/// </summary>
public class NAudioSoundOutputService : ISoundOutputService
{
    /// <summary>
    /// The number of the device that should play the sound (-1 represents the default device)
    /// </summary>
    public int DeviceNumber
    {
        get => _deviceNumber;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "The device number have to be larger or same than -1.");
            _deviceNumber = value;
        }
    }
    private int _deviceNumber;

    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new instance that use the default output device
    /// </summary>
    /// <param name="logger">A logger that is used to log data problems</param>
    public NAudioSoundOutputService(ILogger logger) : this(-1, logger)
    {
    }

    /// <summary>
    /// Creates a new instance that uses the specified device number to play the sounds
    /// </summary>
    /// <param name="deviceNumber">The number of the device that should play all sounds</param>
    /// <param name="logger">A logger that is used to log data problems</param>
    /// <remarks>
    ///     <para>
    ///     -1 represents the default wave out device
    ///     </para>
    ///     <para>
    ///     To enumerate the available devices you have to install the package <c>NAudio.WinForms</c>. See <see href="https://github.com/naudio/NAudio/blob/master/Docs/EnumerateOutputDevices.md#waveout-or-waveoutevent">docs</see>
    ///     </para>
    /// </remarks>
    public NAudioSoundOutputService(int deviceNumber, ILogger logger)
    {
        DeviceNumber = deviceNumber;
        _logger = logger;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task PlaySoundAsync(SoundAsset soundAsset, float volume, float pitch, float panorama, CancellationToken ct = default)
    {
        // argument validation
        ArgumentNullException.ThrowIfNull(soundAsset, nameof(soundAsset));
        if (volume < 0.0f || volume > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(volume), volume, "The volume have to be between 0.0f and 1.0f");
        if (panorama < -1.0f || panorama > 1.0f)
            throw new ArgumentOutOfRangeException(nameof(volume), volume, "The panorama have to be between -1.0f and 1.0f");
        ArgumentNullException.ThrowIfNull(_logger, nameof(_logger));
        if (ct.IsCancellationRequested)
            throw new ArgumentException(string.Format("Could not start playing a sound with an already cancelled {0}", nameof(CancellationToken)));

        // preparation
        using WaveStream waveStream = GetWaveStreamFromAsset(soundAsset, out IDisposable soundAssetDispose);
        LogUnequalMetadata(waveStream, soundAsset, _logger, LogLevel.Warning);

        ISampleProvider sampleProvider = ApplySoundModifier(waveStream, pitch, panorama);

        TaskCompletionSource tcs = new(TaskCreationOptions.LongRunning);
        CancellationTokenRegistration ctr = ct.Register(tcs.SetCanceled);

        // playing
        using WaveOutEvent waveOut = new()
        {
            DeviceNumber = DeviceNumber,
            Volume = volume,
        };

        void waveOut_PlayBackStopped(object? sender, StoppedEventArgs e)
        {
            if (e.Exception is not null)
                _logger.LogError(e.Exception, "An error happened while playing the audio");
            tcs.SetResult();
        }
        waveOut.PlaybackStopped += waveOut_PlayBackStopped;

        try
        {
            waveOut.Init(sampleProvider);
            waveOut.Play();

            await tcs.Task;
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error happened while playing sound {name}", soundAsset.Name);
        }

        // clean up
        finally
        {
            ctr.Unregister();
            waveOut.PlaybackStopped -= waveOut_PlayBackStopped;
            waveOut.Stop();
            soundAssetDispose.Dispose();
        }
    }

    private static WaveStream GetWaveStreamFromAsset(SoundAsset soundAsset, out IDisposable soundAssetDispose)
    {
        Stream audioStream = soundAsset.GetStream();
        soundAssetDispose = audioStream;     // IDisposable is here used as out param because the original audio stream could get used by the wave stream after this method

        return soundAsset.DataFormat switch
        {
            "wav" => new WaveFileReader(audioStream),
            "aiff" => new AiffFileReader(audioStream),
            "mp3" => new Mp3FileReader(audioStream),
            _ => throw new NotSupportedException(string.Format("The audio format '{0}' is not supported by {1}", soundAsset.DataFormat, nameof(NAudioSoundOutputService)))
        };
    }

    private static void LogUnequalMetadata(WaveStream waveStream, SoundAsset soundAsset, ILogger logger, LogLevel logLevel)
    {
        if (waveStream.WaveFormat.SampleRate != soundAsset.Rate)
            logger.Log(logLevel, "The sampling rate of the read audio stream doesn't equal the in the metadata saved sampling rate. Read: {read} Hz; Metadata: {metadata} Hz.", waveStream.WaveFormat.SampleRate, soundAsset.Rate);

        long bytesPerSample = waveStream.WaveFormat.BitsPerSample / 8;
        long sampleCount = waveStream.Length / bytesPerSample;
        if (sampleCount != soundAsset.SampleCount)
            logger.Log(logLevel, "The sampling count of the read audio stream doesn't equal the in the metadata saved sampling count. Read: {read}; Metadata: {metadata}.", sampleCount, soundAsset.SampleCount);
    }

    private static ISampleProvider ApplySoundModifier(WaveStream waveStream, float pitch, float pan)
    {
        ISampleProvider sampleProvider = waveStream.ToSampleProvider();

        if (!pitch.Equals(0f))
        {
            SmbPitchShiftingSampleProvider pitchProvider = new(sampleProvider) { PitchFactor = ++pitch };
            sampleProvider = sampleProvider.FollowedBy(pitchProvider);
        }

        if (!pan.Equals(0f))
        {
            PanningSampleProvider panProvider = new(sampleProvider) { Pan = pan };
            sampleProvider = sampleProvider.FollowedBy(panProvider);
        }

        return sampleProvider;
    }
}
