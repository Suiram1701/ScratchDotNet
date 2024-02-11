using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ScratchDotNet.Core.StageObjects;
using ScratchDotNet.Core.Types.Interfaces;

namespace ScratchDotNet.Core.Execution;

/// <summary>
/// The context of a script execution
/// </summary>
public class ScriptExecutorContext
{
    /// <summary>
    /// The executor of this context
    /// </summary>
    public IStageObject Executor { get; }

    /// <summary>
    /// The stage
    /// </summary>
    public IStage Stage { get; }

    /// <summary>
    /// All figures on the stage
    /// </summary>
    public IEnumerable<IFigure> Figures { get; }

    public Dictionary<string, object> RuntimeData { get; }

    /// <summary>
    /// The logger factory
    /// </summary>
    public ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// A dictionary that contains the instance of a service with the interface type as key
    /// </summary>
    public IReadOnlyDictionary<Type, object> Services { get; }

#pragma warning disable CS8618     // only temporary
    internal ScriptExecutorContext()
#pragma warning restore CS8618
    {
        Figures = Array.Empty<IFigure>();
        RuntimeData = new();
        LoggerFactory = NullLoggerFactory.Instance;
        Services = new Dictionary<Type, object>();
    }
}
