﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.StageObjects;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion.Variables;

/// <summary>
/// Provides the direction of the figure
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Direction : ValueOperatorBase
{
    public override event EventHandler<ValueChangedEventArgs>? OnValueChanged;

    private bool _delegateInitialized;

    private const string _constOpCode = "motion_direction";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public Direction() : base(_constOpCode)
    {
    }

    internal Direction(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    public override Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return Task.FromResult((ScratchTypeBase)new NumberType(0d));
        }

        if (!_delegateInitialized)
        {
            figure.OnDirectionChanged += Figure_OnDirectionChanged;

            logger.LogInformation("Value changed event of block {block} was successfully initialized", BlockId);
            _delegateInitialized = true;
        }

        double result = figure.Direction;
        return Task.FromResult((ScratchTypeBase)new NumberType(result));
    }

    private void Figure_OnDirectionChanged(object? sender, GenericValueChangedEventArgs<double> e)
    {
        if (e.OldValue == e.NewValue)
            return;

        ValueChangedEventArgs eventArgs = new(new NumberType(e.OldValue), new NumberType(e.NewValue));
        OnValueChanged?.Invoke(sender, eventArgs);
    }

    private static string GetDebuggerDisplay() =>
        "direction";
}
