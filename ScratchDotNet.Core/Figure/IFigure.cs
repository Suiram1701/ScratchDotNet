using Newtonsoft.Json;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Figure.Assets;

namespace ScratchDotNet.Core.Figure;

/// <summary>
/// All properties of a figure
/// </summary>
public interface IFigure
{
    /// <summary>
    /// Called when the x position of the figure could have changed
    /// </summary>
    public event Action<double> OnXPositionChanged;

    /// <summary>
    /// Called when the y position of the figure could have changed
    /// </summary>
    public event Action<double> OnYPositionChanged;

    /// <summary>
    /// Called when the direction could have changed
    /// </summary>
    public event Action<double> OnDirectionChanged;

    /// <summary>
    /// The Id of the figure
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// The currently selected costume
    /// </summary>
    [JsonProperty("currentCostume")]
    public int SelectedCostume { get; set; }

    /// <summary>
    /// All available costume assets
    /// </summary>
    [JsonProperty("costumes")]
    public List<CostumeAsset> Costumes { get; set; }

    /// <summary>
    /// The currently set volume of the figure
    /// </summary>
    [JsonProperty("volume")]
    public int Volume { get; set; }

    /// <summary>
    /// All saved sound assets for the figure
    /// </summary>
    [JsonProperty("sounds")]
    public List<SoundAsset> Sounds { get; set; }

    /// <summary>
    /// The Z index of the figure
    /// </summary>
    [JsonProperty("layerOrder")]
    public int LayerOrder { get; set; }

    /// <summary>
    /// Is the figure visible
    /// </summary>
    [JsonProperty("visible")]
    public bool IsVisible { get; set; }

    /// <summary>
    /// The Y coordinate of the figure
    /// </summary>m
    /// <remarks>
    /// The position point is at the center of the figure. 
    /// The Scratch stage has a width from -250 to 250
    /// </remarks>
    [JsonProperty("x")]
    public double X { get; set; }

    /// <summary>
    /// The Y coordinate of the figure
    /// </summary>
    /// <remarks>
    /// The position point is at the center of the figure. 
    /// The Scratch stage has a height from -190 to 190
    /// </remarks>
    [JsonProperty("y")]
    public double Y { get; set; }

    /// <summary>
    /// The size of the figure in percent
    /// </summary>
    [JsonProperty("size")]
    public double Size { get; set; }

    /// <summary>
    /// The height of the figure (relative to the scratch stage)
    /// </summary>
    public double Height { get; }

    /// <summary>
    /// The width of the figure (relative to the scratch stage)
    /// </summary>
    public double Width { get; }

    /// <summary>
    /// The direction of the figure
    /// </summary>
    /// <remarks>
    /// The rotation works a bit different that normaly. 0° start on top and rotates to the bottom until 180°. From now until the top it goes from -179° until 0°
    /// </remarks>
    [JsonProperty("direction")]
    public double Direction { get; set; }

    /// <summary>
    /// Is the figure draggable with the mouse while runtime
    /// </summary>
    [JsonProperty("draggable")]
    public bool Draggable { get; set; }

    /// <summary>
    /// The setting of the rotation
    /// </summary>
    [JsonProperty("rotationStyle")]
    public RotationStyle RotationSetting { get; set; }

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
    /// <remarks>
    /// The rotation works a bit different that normaly. 0° start on top and rotates to the bottom until 180°. From now until the top it goes from -179° until 0°
    /// </remarks>
    /// <param name="degree">The count of degrees</param>
    public void RotateTo(double degree);

    /// <summary>
    /// Set the rotation style to a specified value
    /// </summary>
    /// <param name="rotationStyle">The rotation style</param>
    public void SetRotationStyle(RotationStyle rotationStyle);
}