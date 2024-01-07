using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks.Attributes;
using Scratch.Core.Blocks.Bases;
using Scratch.Core.Enums;
using Scratch.Core.Figure;
using Scratch.Core.Types;
using Scratch.Core.Types.Bases;

namespace Scratch.Core.Blocks.Motion;

/// <summary>
/// Provides selectable values
/// </summary>
[OperatorCode(GotoOpCode)]
[OperatorCode(GlideToOpCode)]
[OperatorCode(PointTowardsOpCode)]
public class TargetReporter : ValueOperatorBase
{
    /// <inheritdoc/>
    /// <remarks>
    /// This will never be get called
    /// </remarks>
    public override event Action OnValueChanged { add { } remove { } }

    /// <summary>
    /// The target of this reporter
    /// </summary>
    public string Target { get; }

    /// <summary>
    /// The data block op code of <see cref="Goto"/>
    /// </summary>
    public const string GotoOpCode = "motion_goto_menu";

    /// <summary>
    /// The data block op code of <see cref="GlideTo"/>
    /// </summary>
    public const string GlideToOpCode = "motion_glideto_menu";

    /// <summary>
    /// The data block op code of <see cref="PointTowards"/>
    /// </summary>
    public const string PointTowardsOpCode = "motion_pointtowards_menu";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The target of this reporter</param>
    /// <param name="opcode">The op code of this reporter (depending on the block that owns this block)</param>
    /// <exception cref="ArgumentException"></exception>
    public TargetReporter(SpecialTarget target, string opcode) : this(target, opcode, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The target of this reporter</param>
    /// <param name="opCode">The op code of this reporter (depending on the block that owns this block)</param>
    /// <param name="blockId">The Id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    public TargetReporter(SpecialTarget target, string opCode, string blockId) : this(MotionHelpers.GetTargetString(target), opCode, blockId)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The target of this reporter</param>
    /// <param name="opCode">The op code of this reporter (depending on the block that owns this block)</param>
    /// <exception cref="ArgumentException"></exception>
    public TargetReporter(IFigure target, string opCode) : this(target, opCode, GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The target figure of this reporter</param>
    /// <param name="opCode">The op code of this reporter (depending on the block that owns this block)</param>
    /// <param name="blockId">The Id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    public TargetReporter(IFigure target, string opCode, string blockId) : this(target.Name, opCode, blockId)
    {
    }

    private TargetReporter(string target, string opCode, string blockId) : base(opCode, blockId)
    {
        if (target != GotoOpCode
            || target != GlideToOpCode
            || target != PointTowardsOpCode)
        {
            string message = string.Format("The specified opCode have to be '{0}', '{1}' or '{2}'.", GotoOpCode, GlideToOpCode, PointTowardsOpCode);
            throw new ArgumentException(message);
        }

        Target = target;
    }

    internal TargetReporter(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        string jsonPath = _opCode switch
        {
            GotoOpCode or
            GlideToOpCode => "fields.TO[0]",
            PointTowardsOpCode => "fields.TOWARDS[0]",
            _ => ThrowUnSupportedOpCodeException()
        };

        Target = blockToken.SelectToken(jsonPath)!.Value<string>()!;
    }

    public override Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default) =>
        Task.FromResult((ScratchTypeBase)new StringType(Target));

    private string ThrowUnSupportedOpCodeException()
    {
        string message = string.Format("The op code {0} isn't supported by the {1}", _opCode, nameof(TargetReporter));
        throw new NotSupportedException(message);
    }
}
