using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scratch.Test.Logger;

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
