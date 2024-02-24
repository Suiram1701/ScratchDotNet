using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types.Interfaces;

namespace ScratchDotNet.Core.Blocks.Bases;

/// <summary>
/// The base for all value operators that own a block
/// </summary>
public abstract class ValueOperatorBase : BlockBase, IValueProvider
{
    public abstract event EventHandler<ValueChangedEventArgs> OnValueChanged;

    /// <inheritdoc/>
    protected ValueOperatorBase(string opcode) : base(opcode)
    {
    }

    /// <inheritdoc/>
    protected ValueOperatorBase(string blockId, string opCode) : base(blockId, opCode)
    {
    }

    /// <inheritdoc/>
    protected internal ValueOperatorBase(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
    }

    public abstract Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default);
}
