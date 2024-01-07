using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks.Interfaces;
using Scratch.Core.Types.Bases;

namespace Scratch.Core.Blocks.Bases;

/// <summary>
/// The base for all value operators that own a block
/// </summary>
public abstract class ValueOperatorBase : BlockBase, IValueProvider
{
    public abstract event Action OnValueChanged;

    /// <inheritdoc/>
    protected ValueOperatorBase(string opcode) : base(opcode)
    {
    }

    /// <inheritdoc/>
    protected ValueOperatorBase(string blockId, string opCode) : base(blockId, opCode)
    {
    }

    /// <inheritdoc/>
    protected ValueOperatorBase(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    public abstract Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default);
}
