using ScratchDotNet.Core.Types.Interfaces;

namespace ScratchDotNet.Core.StageObjects;

/// <summary>
/// Provides all properties a stage needs
/// </summary>
public interface IStage : IStageObject
{
    /// <summary>
    /// A dictionary that conatins data saved by extensions
    /// </summary>
    IDictionary<string, object?> ExtensionData { get; }
}
