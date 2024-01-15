using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;

namespace ScratchDotNet.Core.Blocks.Operator.ConstProviders;

/// <summary>
/// A default implementation of <see cref="IBoolValueProvider"/> that always returns false
/// </summary>
public class EmptyBool : IBoolValueProvider
{
    /// <inheritdoc/>
    /// <remarks>
    /// This will never be called
    /// </remarks>
    public event Action OnValueChanged { add { } remove { } }

    public Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default) =>
        Task.FromResult((ScratchTypeBase)new BooleanType());
}
