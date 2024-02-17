using System.Drawing;

namespace ScratchDotNet.Core.Services.Interfaces;

/// <summary>
/// An interface that provides every mouse and keyboard input
/// </summary>
public interface IInputProviderService
{
    /// <summary>
    /// Gets the mouse position relative to the stage
    /// </summary>
    /// <returns>The position</returns>
    public Point GetMousePosition();
}
