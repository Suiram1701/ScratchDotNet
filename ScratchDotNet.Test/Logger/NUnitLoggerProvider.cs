using Microsoft.Extensions.Logging;

namespace ScratchDotNet.Test.Logger;

/// <summary>
/// A logger provider for <see cref="NUnitLogger"/>
/// </summary>
internal class NUnitLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new NUnitLogger();
    }

    public void Dispose()
    {
    }
}
