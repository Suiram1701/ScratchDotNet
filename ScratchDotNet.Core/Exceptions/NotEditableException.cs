using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Exceptions;

/// <summary>
/// An exceptions that will be thrown when an attempt will be to change data of a block at runtime
/// </summary>
public class NotEditableException : Exception
{
    /// <summary>
    /// Creates a new instance
    /// </summary>
    public NotEditableException() : base("This operation could done at runtime.")
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="message">The message this excpetion displays</param>
    public NotEditableException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="message">The message this exceptions displays</param>
    /// <param name="innerException">The inner exception</param>
    public NotEditableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
