using Microsoft.Extensions.Logging;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ScratchDotNet.Core.Providers.Interfaces;
using ScratchDotNet.Core.StageObjects.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Providers;

public class DefaultSoundOutputService : ISoundOutputService
{
    /// <summary>
    /// The number of the device that should play the sound (-1 means the default device)
    /// </summary>
    public int DeviceNumber { get; set; }

    /// <summary>
    /// Creates a new instance that use the default output device
    /// </summary>
    public DefaultSoundOutputService() : this(-1)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="deviceNumber">The number of the device that should play all sounds</param>
    public DefaultSoundOutputService(int deviceNumber)
    {
        DeviceNumber = deviceNumber;
    }

    public async Task PlaySoundAsync(SoundAsset soundAsset, float volume, float pitch, float panorama, ILogger logger, CancellationToken ct = default)
    {
        using Stream audioStream = soundAsset.GetStream();
        using WaveStream waveStream = soundAsset.DataFormat switch
        {
            "wav" => new WaveFileReader(audioStream),
            "aiff" => new AiffFileReader(audioStream),
            "mp3" => new Mp3FileReader(audioStream),
            _ => ThrowNotSupportedAudioException(soundAsset.DataFormat, logger)
        };

        ISampleProvider sampleProvider = waveStream.ToSampleProvider();

        // Add the sound modifier if necessary
        if (!volume.Equals(1f))
        {
            VolumeSampleProvider volumeProvider = new(sampleProvider) { Volume = volume };
            sampleProvider = volumeProvider;
        }

        if (!pitch.Equals(0f))
        {
            SmbPitchShiftingSampleProvider pitchProvider = new(sampleProvider) { PitchFactor = ++pitch };
            sampleProvider = pitchProvider;
        }

        if (!panorama.Equals(0f))
        {
            PanningSampleProvider panProvider = new(sampleProvider) { Pan = panorama };
            sampleProvider = panProvider;
        }

        TaskCompletionSource tcs = new(TaskCreationOptions.LongRunning);
        CancellationTokenRegistration ctr = ct.Register(tcs.SetCanceled);

        using WaveOutEvent waveOut = new();
        waveOut.PlaybackStopped += (sender, args) =>
        {
            if (args.Exception is not null)
                logger.LogError(args.Exception, "An error happened while playing the audio");
            tcs.SetResult();
        };

        int deviceNumber = Math.Max(-1, DeviceNumber);
        waveOut.DeviceNumber = deviceNumber;

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
            logger.LogError(ex, "An error happened while playing the sound");
        }
        finally
        {
            ctr.Unregister();
            waveOut.Stop();
        }
    }

    private WaveStream ThrowNotSupportedAudioException(string format, ILogger logger)
    {
        logger.LogError("The audio format '{format}' is not supported by '{provider}'", format, nameof(DefaultSoundOutputService));
        throw new NotSupportedException(string.Format("The file format '{0}' is not supported by '{1}'", format, nameof(DefaultSoundOutputService)));
    }
}
