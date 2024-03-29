﻿using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Data;

/// <summary>
/// Provides the stringified content of a list
/// </summary>
/// <remarks>
/// The stringification is done by joining all values of the list separated by a space
/// </remarks>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ListContent : IValueProvider
{
    public event EventHandler<ValueChangedEventArgs>? OnValueChanged;

    /// <summary>
    /// The reference to the list
    /// </summary>
    public ListRef ListRef { get; }

    private bool _delegateInitialized;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The refgerence to the list</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ListContent(ListRef reference)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));
        ListRef = reference;
    }

    public Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        List? list = context.Executor.Lists.FirstOrDefault(l => l.Id.Equals(ListRef.ListId));
        if (list is null)
        {
            logger.LogError("Could not find list with id {id} and name {name}", ListRef.ListId, ListRef.ListName);
            throw new InvalidOperationException(string.Format("Could not find variable with id {0} and name {1}", ListRef.ListId, ListRef.ListName));
        }

        if (!_delegateInitialized)
        {
            list.OnValueChanged += List_OnValueChanged;

            logger.LogInformation("Value changed event to list {id} was successfully initialized", ListRef.ListId);
            _delegateInitialized = true;
        }

        string result = string.Join(' ', list.Values);
        return Task.FromResult<IScratchType>(new StringValue(result));
    }

    private void List_OnValueChanged(object? s, ValueChangedEventArgs e) =>
        OnValueChanged?.Invoke(s, e);

    private string GetDebuggerDisplay() =>
        string.Format("List {0}", ListRef.ListName);
}
