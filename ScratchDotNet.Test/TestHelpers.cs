using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Blocks;
using System.Runtime.CompilerServices;

namespace ScratchDotNet.Test;

internal static class TestHelpers
{
    public static void DoParseTest(string startId, [CallerMemberName] string? loggerName = null)
    {
        ILogger logger = Setup.LoggerFactory.CreateLogger(loggerName ?? "unknown");

        try
        {
            logger.LogInformation("Start test: {test}", loggerName);
            _ = ScriptExecutor.Create(Setup.BlocksToken, startId, logger);
            logger.LogInformation("Test successfully ended");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error happened while reading the blocks");
            Assert.Fail(ex.Message);
            return;
        }

        Assert.Pass();
    }
}
