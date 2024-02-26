using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.StageObjects;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion.Variables;

/// <summary>
/// Provides the y position of the figure
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class YPosition : ValueOperatorBase
{
    public override event EventHandler<ValueChangedEventArgs>? OnValueChanged;

    private bool _delegateInitialized;

    private const string _constOpCode = "motion_yposition";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public YPosition() : base(_constOpCode)
    {
    }

    internal YPosition(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    public override Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IExecutableFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return Task.FromResult<IScratchType>(new DoubleValue());
        }

        if (!_delegateInitialized)
        {
            figure.OnPositionChanged += Figure_OnPositionChanged;

            logger.LogInformation("Value changed event of block {block} was successfully initialized", BlockId);
            _delegateInitialized = true;
        }

        return Task.FromResult<IScratchType>(new DoubleValue(figure.Y));
    }

    private void Figure_OnPositionChanged(object? sender, PositionChangedEventArgs e)
    {
        if (e.OldPosition.Y == e.NewPosition.Y)
            return;

        ValueChangedEventArgs eventArgs = new(new DoubleValue(e.OldPosition.Y), new DoubleValue(e.NewPosition.Y));
        OnValueChanged?.Invoke(sender, eventArgs);
    }

    private static string GetDebuggerDisplay() =>
        "y position";
}
