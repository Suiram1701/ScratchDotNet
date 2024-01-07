using Microsoft.Extensions.Logging;
using Scratch.Core.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Scratch.Test;

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
