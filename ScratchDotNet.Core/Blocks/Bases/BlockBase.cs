using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Exceptions;
using ScratchDotNet.Core.Helpers;

namespace ScratchDotNet.Core.Blocks.Bases;

/// <summary>
/// The base for every block
/// </summary>
public abstract class BlockBase
{
    /// <summary>
    /// The id of this block
    /// </summary>
    public string BlockId { get; }

    protected readonly string _opCode;

    private readonly bool _atRuntime;

    /// <summary>
    /// Creates a new instance with an automatic generated block id
    /// </summary>
    /// <param name="opCode">The op code of this block</param>
    protected BlockBase(string opCode) : this(BlockHelpers.GenerateBlockId(), opCode)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The id of this block</param>
    /// <param name="opCode">The op code of this block</param>
    /// <exception cref="ArgumentException"></exception>
    protected BlockBase(string blockId, string opCode)
    {
        _atRuntime = false;

        ArgumentException.ThrowIfNullOrEmpty(blockId, nameof(blockId));

        BlockId = blockId;
        _opCode = opCode;
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The id of this block</param>
    /// <param name="blockToken">The JToken to read the block from</param>
    protected internal BlockBase(string blockId, JToken blockToken)
    {
        BlockId = blockId;
        _opCode = blockToken["opcode"]!.Value<string>()!;

        BlockConstructionHelper helper = new(this);
        helper.ConstructInputs(blockToken);
        helper.ConstructSubstacks(blockToken);

        _atRuntime = true;
    }

    /// <summary>
    /// Throws an exception when <see cref="_atRuntime"/> is <c>true</c>
    /// </summary>
    /// <exception cref="NotEditableException"></exception>
    protected void ThrowAtRuntime()
    {
        if (!_atRuntime)
            return;
        throw new NotEditableException("The value providers of a block isn't changeable at runtime or after first execution.");
    }
}
