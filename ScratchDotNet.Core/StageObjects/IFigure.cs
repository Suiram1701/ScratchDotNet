using ScratchDotNet.Core.Enums;

namespace ScratchDotNet.Core.StageObjects;

/// <summary>
/// Provides all properties of a figure
/// </summary>
public interface IFigure : IStageObject
{
    /// <summary>
    /// Is the figure visible
    /// </summary>
    public bool IsVisible { get; }

    /// <summary>
    /// The Y coordinate of the figure
    /// </summary>
    public double X { get; }

    /// <summary>
    /// The Y coordinate of the figure
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// The size of the figure in percent
    /// </summary>
    public double Size { get; }

    /// <summary>
    /// The height of the figure
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The width of the figure
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The direction of the figure
    /// </summary>
    public double Direction { get; }

    /// <summary>
    /// Is the figure draggable with the mouse while runtime
    /// </summary>
    public bool Draggable { get; }

    /// <summary>
    /// The setting of the rotation
    /// </summary>
    public RotationStyle RotationSetting { get; }
}