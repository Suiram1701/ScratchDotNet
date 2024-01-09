using ScratchDotNet.Core.Enums;

namespace ScratchDotNet.Core.Blocks.Interfaces;

/// <summary>
/// Provides an value provider with an const value
/// </summary>
internal interface IConstProvider : IValueProvider
{
    /// <summary>
    /// The type of the const value
    /// </summary>
    public DataType DataType { get; internal set; }
}
