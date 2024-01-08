using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks;
using Scratch.Core.Blocks.Attributes;
using Scratch.Core.Blocks.Bases;
using Scratch.Core.Types;
using Scratch.Core.Types.Bases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Motion.Variables;

/// <summary>
/// Provides the y position of the figure
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class YPosition : ValueOperatorBase
{
    public override event Action? OnValueChanged;

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

    public override Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Figure is null)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return Task.FromResult((ScratchTypeBase)new NumberType(0d));
        }

        if (!_delegateInitialized)     // Intialize the delegate
        {
            context.Figure.OnYPositionChanged += ValueChanged;
            _delegateInitialized = true;
        }

        return Task.FromResult((ScratchTypeBase)new NumberType(context.Figure.Y));
    }

    private void ValueChanged(double value)
    {
        if (OnValueChanged is not null)
            OnValueChanged();
    }

    private static string GetDebuggerDisplay() =>
        "y position";
}
