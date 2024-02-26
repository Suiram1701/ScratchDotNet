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
/// Turns a list visible / invisible for the user
/// </summary>
[ExecutionBlockCode(_constShowOpCode)]
[ExecutionBlockCode(_constHideOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class SetListVisibility : ListExecutionBase
{
    /// <summary>
    /// Indicates whether this block should turn the visibility on or off
    /// </summary>
    public Visibility Visibility { get; }

    private const string _constShowOpCode = "data_showlist";
    private const string _constHideOpCode = "data_hidelist";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="visibility">The visibility to set</param>
    /// <param name="reference">The reference to the list</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SetListVisibility(Visibility visibility, ListRef reference) : this(visibility, reference, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="visibility">The visibility to set</param>
    /// <param name="reference">The reference to the list</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SetListVisibility(Visibility visibility, ListRef reference, string blockId) : base(reference, GetOpCodeFromVisibility(visibility), blockId)
    {
        ArgumentNullException.ThrowIfNull(visibility, nameof(visibility));
        Visibility = visibility;
    }

    internal SetListVisibility(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        Visibility = _opCode switch
        {
            _constShowOpCode => Visibility.Visible,
            _constHideOpCode => Visibility.Hidden,
            _ => throw new NotSupportedException("The specified op code isn't supported.")
        };
    }

    protected override Task ExecuteInternalAsync(ScriptExecutorContext context, List list, ILogger logger, CancellationToken ct = default)
    {
        // TODO: Implement list monitor
        throw new NotImplementedException();
    }

    private static string GetOpCodeFromVisibility(Visibility visibility)
    {
        return visibility switch
        {
            Visibility.Visible => _constShowOpCode,
            Visibility.Hidden => _constHideOpCode,
            _ => throw new NotSupportedException("The specified option isn't supported.")
        };
    }

    private string GetDebuggerDisplay() =>
        string.Format("{0} list {1}", Visibility.ToString().ToLower(), ListRef.ListName);
}
