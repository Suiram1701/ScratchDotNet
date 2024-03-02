using ScratchDotNet.Core.EventArgs;
using System.Drawing;

namespace ScratchDotNet.Core.StageObjects;

/// <summary>
/// An interface that provides all methods for all objects that are by the blocks
/// </summary>
public interface IExecutableStageObject : IStageObject
{
    /// <summary>
    /// Called when the volume of the object could have been changed
    /// </summary>
    public event EventHandler<GenericValueChangedEventArgs<double>> OnVolumeChanged;

    /// <summary>
    /// Sets the volume to a specified value in percent
    /// </summary>
    /// <param name="volume">The value to set</param>
    public void SetSoundVolume(double volume);

    /// <summary>
    /// Returns the rendered size of the currently rendered custume
    /// </summary>
    /// <returns>The size</returns>
    public SizeF GetRenderedSize();
}
