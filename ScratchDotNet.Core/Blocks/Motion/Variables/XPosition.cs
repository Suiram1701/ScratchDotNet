using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.StageObjects;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion.Variables;

/// <summary>
/// Provides the x position of the figure
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class XPosition : ValueOperatorBase
{
    public override event Action? OnValueChanged;

    private bool _delegateInitialized;

    private const string _constOpCode = "motion_xposition";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public XPosition() : base(_constOpCode)
    {
    }

    internal XPosition(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    public override Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return Task.FromResult((ScratchTypeBase)new NumberType());
        }

        if (!_delegateInitialized)
        {
            figure.OnXPositionChanged += ValueChanged;

            logger.LogInformation("Value changed event of block {block} was successfully initialized", BlockId);
            _delegateInitialized = true;
        }

        return Task.FromResult((ScratchTypeBase)new NumberType(figure.X));
    }

    private void ValueChanged(double value)
    {
        if (OnValueChanged is not null)
            OnValueChanged();
    }

    private static string GetDebuggerDisplay() =>
        "x position";
}
