using Scratch.Core.Enums;
using Scratch.Core.Figure.Assets;

namespace Scratch.Core.Figure;

/// <summary>
/// A default implementation of <see cref="IFigure"/>
/// </summary>
public sealed class Figure : IFigure
{
    public string Name { get; set; }

    public int SelectedCostume { get; set; }

    public List<CostumeAsset> Costumes { get; set; }

    public int Volume { get; set; }

    public List<SoundAsset> Sounds { get; set; }

    public int LayerOrder { get; set; }

    public bool IsVisible { get; set; }

    public double X { get; set; }

    public double Y { get; set; }

    public double Size { get; set; }

    public double Height { get; }

    public double Width { get; }

    public double Direction { get; set; }

    public bool Draggable { get; set; }

    public RotationStyle RotationSetting { get; set; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public Figure()
    {
        Name = string.Empty;
        Costumes = new();
        Sounds = new();
    }

    public void MoveTo(double x, double y)
    {
        X = x;
        Y = y;
    }

    public Task GlideToAsync(double x, double y, TimeSpan time, CancellationToken ct = default)
    {
        MoveTo(x, y);
        return Task.CompletedTask;
    }

    public void RotateTo(double degree)
    {
        while (degree > 180)
            degree -= 360;
        while (degree <= -180)
            degree += 360;

        Direction = degree;
    }

    public void SetRotationStyle(RotationStyle style)
    {
        RotationSetting = style;
    }
}