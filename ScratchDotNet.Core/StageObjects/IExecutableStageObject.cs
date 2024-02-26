using ScratchDotNet.Core.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
