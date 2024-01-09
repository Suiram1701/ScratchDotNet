﻿using ScratchDotNet.Core.Types;

namespace ScratchDotNet.Core.Blocks.Interfaces;

/// <summary>
/// An interface that provides a boolean value
/// </summary>
/// <remarks>
/// An implementation of this always have to return <see cref="BooleanType"/> at <see cref="IValueProvider.GetResultAsync(ScriptExecutorContext, Microsoft.Extensions.Logging.ILogger, CancellationToken)"/>
/// </remarks>
public interface IBoolValueProvider : IValueProvider
{
}
