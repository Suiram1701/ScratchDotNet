namespace ScratchDotNet.Core.Enums;

/// <summary>
/// The ids of the scratch internaly used data types
/// </summary>
public enum DataType
{
    /// <summary>
    /// A normal number (here is <see cref="double"/> used)
    /// </summary>
    Number = 4,

    /// <summary>
    /// A positive number (here is <see cref="double"/> used)
    /// </summary>
    PositiveNumber = 5,

    /// <summary>
    /// A normal number (here also <see cref="double"/> used but parsed as <see cref="int"/>)
    /// </summary>
    Integer = 7,

    /// <summary>
    /// A normal positive number (here also <see cref="double"/> used but parsed as <see cref="int"/>)
    /// </summary>
    PositiveInteger = 6,

    /// <summary>
    /// A normal number interpretet as angle (-179° - 180°)(here is <see cref="double"/> used)
    /// </summary>
    Angle = 8,

    /// <summary>
    /// A hex color with the '#' prefix
    /// </summary>
    Color = 9,

    /// <summary>
    /// A normal string
    /// </summary>
    String = 10,

    /// <summary>
    /// A broadcast where figures can send messages each other
    /// </summary>
    Broadcast = 11,

    /// <summary>
    /// A variable
    /// </summary>
    Variable = 12,

    /// <summary>
    /// A list
    /// </summary>
    List = 13
}
