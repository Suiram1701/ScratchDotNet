using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.EventArgs;

namespace ScratchDotNet.Core.StageObjects;

/// <summary>
/// An interface that provides all methods for figure that are by the blocks
/// </summary>
public interface IExecutableFigure : IExecutableStageObject, IFigure
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
    /// Moves the figure instandly to a specified point
    /// </summary>
    /// <remarks>
    /// The position point is at the center of the figure
    /// </remarks>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    public void MoveTo(double x, double y);

    /// <summary>
    /// Glides the figure to a specified position
    /// </summary>
    /// <param name="x">The x position</param>
    /// <param name="y">The y position</param>
    /// <param name="time">The time that the figure needs to move there</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The async task</returns>
    public Task GlideToAsync(double x, double y, TimeSpan time, CancellationToken ct = default);

    /// <summary>
    /// Set direction of the figure to a specified count of degrees
    /// </summary>
    /// <param name="degree">The count of degrees</param>
    public void RotateTo(double degree);

    /// <summary>
    /// Set the rotation style to a specified value
    /// </summary>
    /// <param name="rotationStyle">The rotation style</param>
    public void SetRotationStyle(RotationStyle rotationStyle);
}
