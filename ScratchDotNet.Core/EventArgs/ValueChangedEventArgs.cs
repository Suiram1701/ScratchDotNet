using ScratchDotNet.Core.Types.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.EventArgs;

/// <summary>
/// Event args that contains information about a value that have been changed
/// </summary>
public class ValueChangedEventArgs : System.EventArgs
{
    /// <summary>
    /// The old value
    /// </summary>
    public ScratchTypeBase? OldValue { get; }

    /// <summary>
    /// The new value
    /// </summary>
    public ScratchTypeBase? NewValue { get; }

    /// <summary>
    /// An empyt instance of this event args that contains no data
    /// </summary>
    public static readonly new ValueChangedEventArgs Empty;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="oldValue">The old value</param>
    /// <param name="newValue">The new value</param>
    /// <param name="action">The action of this change</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ValueChangedEventArgs(ScratchTypeBase? oldValue, ScratchTypeBase? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    static ValueChangedEventArgs()
    {
        Empty = new(null, null);
    }
}
