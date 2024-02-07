using Microsoft.Extensions.Logging;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types.Bases;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Data;

/// <summary>
/// Provides the content of a variable
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class VariableContent : IValueProvider
{
    public event EventHandler<ValueChangedEventArgs>? OnValueChanged;

    /// <summary>
    /// A reference to the variable whose value should read out
    /// </summary>
    public VariableRef VariableRef { get; }

    private bool _delegateInitialized;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">The variable whose value should read out</param>
    public VariableContent(VariableRef reference)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));
        VariableRef = reference;
    }

    public Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        Variable? variable = context.Executor.Variables.FirstOrDefault(var => var.Id.Equals(VariableRef.VarId));
        if (variable is null)
        {
            logger.LogError("Could not find variable with id {id} and name {name}", VariableRef.VarId, VariableRef.VarName);
            throw new InvalidOperationException(string.Format("Could not find variable with id {0} and name {1}", VariableRef.VarId, VariableRef.VarName));
        }

        if (!_delegateInitialized)
        {
            variable.OnValueChanged += Variable_OnValueChanged;

            logger.LogInformation("Value changed event to variable {id} was successfully initialized", VariableRef.VarId);
            _delegateInitialized = true;
        }

        return Task.FromResult(variable.Value);
    }

    private void Variable_OnValueChanged(object? sender, ValueChangedEventArgs e) =>
        OnValueChanged?.Invoke(sender, e);

    private string GetDebuggerDisplay() =>
        string.Format("var {0}", VariableRef.VarName);
}
