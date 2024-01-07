using System.Drawing;

namespace Scratch.Core.DataProviders;

/// <summary>
/// A data provider for the actions
/// </summary>
public class PhysicalDataProvider
{
    /// <summary>
    /// A delegate to get the mouse position
    /// </summary>
    /// <returns>The position relative inside the stage</returns>
    public delegate Point PositionDelegate();

    public PositionDelegate MousePosition { get; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="getPosition">The method to get the mouse position</param>
    public PhysicalDataProvider(PositionDelegate getPosition)
    {
        MousePosition = getPosition;
    }
}