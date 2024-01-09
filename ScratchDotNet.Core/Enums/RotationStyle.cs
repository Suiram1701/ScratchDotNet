using ScratchDotNet.Core.Attributes;

namespace ScratchDotNet.Core.Enums;

/// <summary>
/// The different options of a figures rotation behavior
/// </summary>
public enum RotationStyle
{
    /// <summary>
    /// Full rotation possible
    /// </summary>
    [EnumName("all around")]
    AllRound,

    /// <summary>
    /// Rotation isn't possible in single degree. The figure can only look to the left or right
    /// </summary>
    [EnumName("left-right")]
    LeftRight,

    /// <summary>
    /// Rotation is locked
    /// </summary>
    [EnumName("don't rotate")]
    Locked
}
