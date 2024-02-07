using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.EventArgs;

/// <summary>
/// Event args that are used to inform about a changed positition
/// </summary>
public class PositionChangedEventArgs : System.EventArgs
{
    /// <summary>
    /// The old position
    /// </summary>
    public PointF OldPosition { get; }

    /// <summary>
    /// The new position
    /// </summary>
    public PointF NewPosition { get; }

    /// <summary>
    /// An empty instance
    /// </summary>
    public static readonly new PositionChangedEventArgs Empty;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="oldPosition">The old position</param>
    /// <param name="newPosition">The new position</param>
    public PositionChangedEventArgs(PointF oldPosition, PointF newPosition)
    {
        OldPosition = oldPosition;
        NewPosition = newPosition;
    }

    static PositionChangedEventArgs()
    {
        Empty = new(default, default);
    }
}
