using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.StageObjects;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion;

/// <summary>
/// Changes the rotation style of the figure
/// </summary>
/// <remarks>
/// This block have to got executed by a figure
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class SetRotationStyle : ExecutionBlockBase
{
    /// <summary>
    /// The rotation style to set
    /// </summary>
    public RotationStyle RotationStyle { get; }

    private const string _constOpCode = "motion_setrotationstyle";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="rotationStyle">The rotation style to set</param>
    public SetRotationStyle(RotationStyle rotationStyle) : this(rotationStyle, BlockHelpers.GenerateBlockId())
    {
        RotationStyle = rotationStyle;
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="rotationStyle">The rotation style to set</param>
    /// <param name="blockId">The Id of this block</param>
    public SetRotationStyle(RotationStyle rotationStyle, string blockId) : base(_constOpCode, blockId)
    {
        RotationStyle = rotationStyle;
    }

    internal SetRotationStyle(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        string styleString = blockToken.SelectToken("fields.STYLE[0]")!.Value<string>()!;
        RotationStyle = EnumNameAttributeHelpers.ParseEnumWithName<RotationStyle>(styleString);
    }

    protected internal override Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IExecutableFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return Task.CompletedTask;
        }

        figure.SetRotationStyle(RotationStyle);
        return Task.CompletedTask;
    }

    private string GetDebuggerDisplay()
    {
        string style = EnumNameAttributeHelpers.GetNames<RotationStyle>()
            .FirstOrDefault(kv => kv.Value == RotationStyle).Key;
        return string.Format("Set rotation style: {0}", style);
    }
}
