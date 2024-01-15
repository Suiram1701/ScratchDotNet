using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Execution;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ScratchDotNet.Test;

internal static class TestHelpers
{
    public static void DoParseTest(string startId, [CallerMemberName] string? loggerName = null)
    {
        Stopwatch sw = new();
        ILogger logger = Setup.LoggerFactory.CreateLogger(loggerName ?? "unknown");

        try
        {
            logger.LogInformation("Start test: {test}", loggerName);

            sw.Start();
            _ = ScriptExecutor.Create(Setup.BlockTokens, startId, logger);
            sw.Stop();

            logger.LogInformation("Test successfully ended");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error happened while reading the blocks");
            Assert.Fail(ex.Message);
            return;
        }

        Assert.Pass("Needed time: {0}ms", sw.ElapsedMilliseconds);
    }
}
