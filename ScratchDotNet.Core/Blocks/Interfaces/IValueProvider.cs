using Microsoft.Extensions.Logging;
using Scratch.Core.Types.Bases;

namespace Scratch.Core.Blocks.Interfaces;

/// <summary>
/// An interface that provides evaluation of a value operator
/// </summary>
public interface IValueProvider
{
    /// <summary>
    /// Indicates that the result of <see cref="GetResultAsync(ScriptExecutorContext, ILogger, CancellationToken)"/> could have changed
    /// </summary>
    public event Action OnValueChanged;

    /// <summary>
    /// Evaluate the result
    /// </summary>
    /// <param name="context">The execution context</param>
    /// <param name="logger">The logger</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The result</returns>
    public Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default);
}
