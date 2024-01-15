using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;
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

            logger.LogInformation("Delegate of block {block} was successfully initialized", BlockId);
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
