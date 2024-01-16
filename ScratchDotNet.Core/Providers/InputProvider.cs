using System.Drawing;

namespace ScratchDotNet.Core.DataProviders;

/// <summary>
/// A data provider mouse and keyboard
/// </summary>
public class InputProvider
{
    /// <summary>
    /// A delegate to get the mouse position
    /// </summary>
    /// <returns>The position relative inside the stage</returns>
    public delegate Point PositionDelegate();

    /// <summary>
    /// Get mouse position
    /// </summary>
    public PositionDelegate MousePosition { get; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="getPosition">The method to get the mouse position</param>
    public InputProvider(PositionDelegate getPosition)
    {
        MousePosition = getPosition;
    }
}