using ScratchDotNet.Core.Attributes;

namespace ScratchDotNet.Core.Enums;

/// <summary>
/// Different scopes of <see cref="Blocks.Control.StopScript"/>
/// </summary>
public enum StopScope
{
    /// <summary>
    /// Stops the currently executed script
    /// </summary>
    [EnumName("this script")]
    This,

    /// <summary>
    /// Stops every script of the project
    /// </summary>
    [EnumName("all")]
    All,

    /// <summary>
    /// Stops all scripts of the executor except the current script
    /// </summary>
    [EnumName("other scripts in sprite")]
    OtherScripts
}
