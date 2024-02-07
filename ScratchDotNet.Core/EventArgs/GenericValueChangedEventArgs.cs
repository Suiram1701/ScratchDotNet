using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.EventArgs;

/// <summary>
/// Generic event args that provides an old- and new value
/// </summary>
/// <typeparam name="TValue">The type of the value</typeparam>
public class GenericValueChangedEventArgs<TValue> : System.EventArgs
{
    /// <summary>
    /// The old value
    /// </summary>
    public TValue OldValue { get; }

    /// <summary>
    /// The new value
    /// </summary>
    public TValue NewValue { get; }

    /// <summary>
    /// An empty instance
    /// </summary>
    public static readonly new GenericValueChangedEventArgs<TValue> Empty;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="oldValue">The old value</param>
    /// <param name="newValue">The new value</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GenericValueChangedEventArgs(TValue oldValue, TValue newValue)
    {
        ArgumentNullException.ThrowIfNull(oldValue, nameof(oldValue));
        ArgumentNullException.ThrowIfNull(newValue, nameof(newValue));

        OldValue = oldValue;
        NewValue = newValue;
    }

    static GenericValueChangedEventArgs()
    {
        Empty = new(default!, default!);
    }
}
