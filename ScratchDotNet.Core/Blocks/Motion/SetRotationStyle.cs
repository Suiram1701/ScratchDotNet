using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks.Attributes;
using Scratch.Core.Blocks.Bases;
using Scratch.Core.Enums;
using System.Diagnostics;

namespace Scratch.Core.Blocks.Motion;

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
    public SetRotationStyle(RotationStyle rotationStyle) : this(rotationStyle, GenerateBlockId())
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

    protected override Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Figure is null)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return Task.CompletedTask;
        }

        context.Figure.SetRotationStyle(RotationStyle);
        return Task.CompletedTask;
    }

    private string GetDebuggerDisplay()
    {
        string style = EnumNameAttributeHelpers.GetNames<RotationStyle>()
            .FirstOrDefault(kv => kv.Value == RotationStyle).Key;
        return string.Format("Set rotation style: {0}", style);
    }
}
