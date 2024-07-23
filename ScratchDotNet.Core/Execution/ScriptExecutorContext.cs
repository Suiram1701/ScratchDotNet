using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ScratchDotNet.Core.StageObjects;

namespace ScratchDotNet.Core.Execution;

/// <summary>
/// The context of a script execution
/// </summary>
public class ScriptExecutorContext
{
    /// <summary>
    /// The executor of this context
    /// </summary>
    public IExecutableStageObject Executor { get; }

    /// <summary>
    /// The stage
    /// </summary>
    public IExecutableStage Stage { get; }

    /// <summary>
    /// All figures on the stage
    /// </summary>
    public IEnumerable<IExecutableFigure> Figures { get; }

    public Dictionary<string, object> RuntimeData { get; }

    /// <summary>
    /// The logger factory
    /// </summary>
    public ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// A provider for services during the execution.
    /// </summary>
    public IServiceProvider ServicesProvider { get; }

#pragma warning disable CS8618     // only temporary
    internal ScriptExecutorContext()
#pragma warning restore CS8618
    {
        Figures = [];
        RuntimeData = [];
        LoggerFactory = NullLoggerFactory.Instance;
    }
}
