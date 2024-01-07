using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scratch.Core.Types;

namespace Scratch.Core.Blocks.Interfaces;

/// <summary>
/// An interface that provides a boolean value
/// </summary>
/// <remarks>
/// An implementation of this always have to return <see cref="BooleanType"/> at <see cref="IValueProvider.GetResultAsync(Scratch.Core.Blocks.ScriptExecutorContext, Microsoft.Extensions.Logging.ILogger, CancellationToken)"/>
/// </remarks>
public interface IBoolValueProvider : IValueProvider
{
}
