using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;

namespace ScratchDotNet.Core.Extensions;

public static class IBoolProviderExtensions
{
    /// <summary>
    /// Returns the result of <see cref="IValueProvider.GetResultAsync(ScriptExecutorContext, ILogger, CancellationToken)"/> casted as <see cref="BooleanValue"/>
    /// </summary>
    /// <param name="provider">The provider instance</param>
    /// <param name="context">The execution context</param>
    /// <param name="logger">The logger to use</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The result as a task</returns>
    public static async Task<BooleanValue> GetBooleanResultAsync(this IBoolValueProvider provider, ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        try
        {
            IScratchType result = await provider.GetResultAsync(context, logger, ct);
            return (BooleanValue)result;
        }
        catch (InvalidCastException ex)
        {
            logger.LogError(ex, "An implementation of {boolProvider} returned a result that isn't castable to {bool}.", nameof(IBoolValueProvider), nameof(BooleanValue));
            throw new NotImplementedException();     // TODO: Implement empty value
        }
    }
}
