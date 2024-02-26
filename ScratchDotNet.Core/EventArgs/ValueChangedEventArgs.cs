using ScratchDotNet.Core.Types.Interfaces;

namespace ScratchDotNet.Core.EventArgs;

/// <summary>
/// Event args that contains information about a value that have been changed
/// </summary>
/// <param name="oldValue">The old value</param>
/// <param name="newValue">The new value</param>
/// <exception cref="ArgumentNullException"></exception>
public class ValueChangedEventArgs(IScratchType? oldValue, IScratchType? newValue) : System.EventArgs
{
    /// <summary>
    /// The old value
    /// </summary>
    public IScratchType? OldValue { get; } = oldValue;

    /// <summary>
    /// The new value
    /// </summary>
    public IScratchType? NewValue { get; } = newValue;

    /// <summary>
    /// An empyt instance of this event args that contains no data
    /// </summary>
    public static readonly new ValueChangedEventArgs Empty;

    static ValueChangedEventArgs()
    {
        Empty = new(null, null);
    }
}
