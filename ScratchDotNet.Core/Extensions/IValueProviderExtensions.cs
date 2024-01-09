using Microsoft.Extensions.Logging.Abstractions;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Types.Bases;

namespace ScratchDotNet.Core.Extensions;

internal static class IValueProviderExtensions
{
    /// <summary>
    /// Evaluate the result without any context
    /// </summary>
    /// <remarks>
    /// This may be end up with exceptions
    /// </remarks>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="provider"></param>
    /// <returns>The result</returns>
    public static ScratchTypeBase GetDefaultResult(this IValueProvider provider) =>
        provider.GetResultAsync(new(), NullLogger.Instance, CancellationToken.None).Result;
}
