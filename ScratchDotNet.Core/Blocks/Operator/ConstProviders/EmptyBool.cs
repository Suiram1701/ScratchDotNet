using Microsoft.Extensions.Logging;
using Scratch.Core.Blocks.Interfaces;
using Scratch.Core.Types;
using Scratch.Core.Types.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scratch.Core.Blocks.Operator.ConstProviders;

/// <summary>
/// A default implementation of <see cref="IBoolValueProvider"/> that always returns false
/// </summary>
public class EmptyBool : IBoolValueProvider
{
    /// <inheritdoc/>
    /// <remarks>
    /// This will never be called
    /// </remarks>
    public event Action OnValueChanged { add { } remove { } }

    public Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default) =>
        Task.FromResult((ScratchTypeBase)new BooleanType());
}
