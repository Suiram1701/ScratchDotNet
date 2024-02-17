using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Types;

/// <summary>
/// Represents a empty value in the scratch type system
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class EmptyValue : IScratchType, IValueProvider
{
    /// <inheritdoc/>
    /// <remarks>
    /// This will be never get called
    /// </remarks>
    public event EventHandler<ValueChangedEventArgs> OnValueChanged { add { } remove { } }

    public override bool Equals(object? obj) =>
        Equals(obj as IScratchType);

    public virtual bool Equals(IScratchType? other) =>
        other is EmptyValue;

    public virtual int CompareTo(IScratchType? other)
    {
        // Empty is always smaller than another value excpet when the other value is also an empty value
        if (other is EmptyValue)
            return 0;
        return -1;
    }

    public virtual DoubleValue ConvertToDoubleValue() =>
        new();

    public virtual StringValue ConvertToStringValue() =>
        new();

    public virtual Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default) =>
        Task.FromResult<IScratchType>(new StringValue());

    public static bool operator ==(EmptyValue left, EmptyValue right) =>
        left.Equals(right);

    public static bool operator !=(EmptyValue left, EmptyValue right) =>
        !left.Equals(right);

    private static string GetDebuggerDisplay() =>
        "Empty value";
}
