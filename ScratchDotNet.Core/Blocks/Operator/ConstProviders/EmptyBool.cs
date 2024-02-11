using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;

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
    public event EventHandler<ValueChangedEventArgs> OnValueChanged { add { } remove { } }

    public Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default) =>
        Task.FromResult<IScratchType>(new BooleanValue());
}
