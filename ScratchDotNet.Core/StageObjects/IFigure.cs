using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.EventArgs;

namespace ScratchDotNet.Core.StageObjects;

/// <summary>
/// Provides all properties of a figure
/// </summary>
public interface IFigure : IStageObject
{
    /// <summary>
    /// Called when the position of the figure could have changed
    /// </summary>
    public event EventHandler<PositionChangedEventArgs> OnPositionChanged;

    /// <summary>
    /// Called when the direction could have changed
    /// </summary>
    public event EventHandler<GenericValueChangedEventArgs<double>> OnDirectionChanged;

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

    /// <summary>
    /// Moves the figure instandly to a specified point
    /// </summary>
    /// <remarks>
    /// The position point is at the center of the figure
    /// </remarks>
    /// <param name="x">The x coordinate. The value have to be between -250 to 250</param>
    /// <param name="y">The y coordinate. The value have to be between -190 to 190</param>
    public void MoveTo(double x, double y);

    /// <summary>
    /// Slides the figure to a specified position
    /// </summary>
    /// <param name="x">The x position</param>
    /// <param name="y">The y position</param>
    /// <param name="time">The time that the figure needs to move there</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The async task</returns>
    public Task GlideToAsync(double x, double y, TimeSpan time, CancellationToken ct = default);

    /// <summary>
    /// Set the figure to a specified count of degrees
    /// </summary>
    /// <param name="degree">The count of degrees</param>
    public void RotateTo(double degree);

    /// <summary>
    /// Set the rotation style to a specified value
    /// </summary>
    /// <param name="rotationStyle">The rotation style</param>
    public void SetRotationStyle(RotationStyle rotationStyle);
}