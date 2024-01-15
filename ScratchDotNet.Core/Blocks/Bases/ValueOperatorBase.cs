using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types.Bases;

namespace ScratchDotNet.Core.Blocks.Bases;

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
