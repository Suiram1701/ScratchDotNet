using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ScratchDotNet.Core.DataProviders;
using ScratchDotNet.Core.Figure;

namespace ScratchDotNet.Core.Blocks;

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
    /// The logger factory
    /// </summary>
    public ILoggerFactory LoggerFactory { get; }

    /// <summary>
    /// A data provider for physical input data
    /// </summary>
    public PhysicalDataProvider PhysicalDataProvider { get; }

    /// <summary>
    /// An empty instance
    /// </summary>
    internal ScriptExecutorContext()
    {
        Figures = Array.Empty<IFigure>();
        LoggerFactory = NullLoggerFactory.Instance;
        PhysicalDataProvider = new(() => new(0, 0));
    }

    internal ScriptExecutorContext(IFigure? figure, IEnumerable<IFigure> figures, ILoggerFactory loggerFactory, PhysicalDataProvider physicalDataProvider)
    {
        Figure = figure;
        Figures = figures;
        LoggerFactory = loggerFactory;
        PhysicalDataProvider = physicalDataProvider;
    }
}
