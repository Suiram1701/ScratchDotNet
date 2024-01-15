using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Data;

/// <summary>
/// Turns a variable visible / invisible for the user
/// </summary>
[ExecutionBlockCode(_showConstOpCode)]
[ExecutionBlockCode(_hideConstOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class SetVariableVisibility : VariableExecutionBase
{
    /// <summary>
    /// Indicates whether this block should turn the visibility on or off
    /// </summary>
    public Visibility Visibility { get; }

    private const string _showConstOpCode = "data_showvariable";
    private const string _hideConstOpCode = "data_hidevariable";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="visibility">The visibility to set</param>
    /// <param name="reference">A reference to the variable where the visibility have to applied to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariableVisibility(Visibility visibility, VariableRef reference) : this(visibility, reference, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="visibility">The visibility to set</param>
    /// <param name="reference">A reference to the variable where the visibility have to applied to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SetVariableVisibility(Visibility visibility, VariableRef reference, string blockId) : base(reference, blockId, GetOpCodeFromVisibility(visibility))
    {
        Visibility = visibility;
    }

    internal SetVariableVisibility(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        Visibility = _opCode switch
        {
            _showConstOpCode => Visibility.Visible,
            _hideConstOpCode => Visibility.Hidden,
            _ => throw new NotSupportedException("The specified op code isn't supported.")
        };
    }

    protected override Task ExecuteInternalAsync(ScriptExecutorContext context, Variable variable, ILogger logger, CancellationToken ct = default)
    {
        // TODO: Implement variable monitor
        throw new NotImplementedException();
    }

    private static string GetOpCodeFromVisibility(Visibility visibility) => visibility switch
    {
        Visibility.Visible => _showConstOpCode,
        Visibility.Hidden => _hideConstOpCode,
        _ => throw new NotSupportedException("The specified visibility options isn't supported.")
    };

    private string GetDebuggerDisplay() =>
        string.Format("{0} var {1}", Visibility.ToString().ToLower(), VariableRef.VarName);
}