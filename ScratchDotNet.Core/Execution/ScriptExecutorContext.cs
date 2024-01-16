using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.DataProviders;
using ScratchDotNet.Core.Figure;

namespace ScratchDotNet.Core.Execution;

/// <summary>
/// The context of a script execution
/// </summary>
public class ScriptExecutorContext
{
    /// <summary>
    /// The figure that execute the script (<see langword="null"/> if no figure execute the script)
    /// </summary>
    public IFigure? Figure { get; }

    /// <summary>
    /// All figures on the stage
    /// </summary>
    public IEnumerable<IFigure> Figures { get; }

    /// <summary>
    /// The variables of the current executor
    /// </summary>
    public IEnumerable<Variable> Variables { get; }

    /// <summary>
    /// The logger factory
    /// </summary>
    public ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// A data provider for physical input data
    /// </summary>
    public InputProvider PhysicalDataProvider { get; }

    internal ScriptExecutorContext()
    {
        Figures = Array.Empty<IFigure>();
        Variables = Array.Empty<Variable>();
        LoggerFactory = NullLoggerFactory.Instance;
        PhysicalDataProvider = new(() => new(0, 0));
    }

    internal ScriptExecutorContext(IFigure? figure, IEnumerable<IFigure> figures, IEnumerable<Variable> variables, ILoggerFactory loggerFactory, InputProvider physicalDataProvider)
    {
        Figure = figure;
        Figures = figures;
        Variables = variables;
        LoggerFactory = loggerFactory;
        PhysicalDataProvider = physicalDataProvider;
    }
}
