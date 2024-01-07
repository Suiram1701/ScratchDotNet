using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scratch.Test.Logger;

/// <summary>
/// A logger that fails the assertion on a <see cref="LogLevel.Error"/> or <see cref="LogLevel.Critical"/>
/// </summary>
internal class NUnitLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull =>
        null;

    public bool IsEnabled(LogLevel logLevel) =>
        true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Debug.WriteLine(formatter(state, exception));

        if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
        {
            string message = "An unexpected error happened";
            if (exception is not null)
            {
                WriteExceptionRecurive(exception);

                message = exception.Message;
                message += Environment.NewLine;
                message += exception.StackTrace;
            }

            Assert.Fail(message);
        }
    }

    private static void WriteExceptionRecurive(Exception exception)
    {
        Debug.Write(exception.GetType().FullName);
        Debug.Write(": ");
        Debug.WriteLine(exception.Message);

        if (exception.InnerException is not null)
            WriteExceptionRecurive(exception.InnerException);
    }
}
