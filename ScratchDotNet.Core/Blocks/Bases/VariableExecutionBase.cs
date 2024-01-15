using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Data;
using ScratchDotNet.Core.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Bases;

/// <summary>
/// The base for blocks that manipulates a variable
/// </summary>
public abstract class VariableExecutionBase : ExecutionBlockBase
{
    /// <summary>
    /// A reference to the target variable to set
    /// </summary>
    public VariableRef VariableRef { get; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">A reference to the variable to set</param>
    /// <param name="opCode">The op code of this block</param>
    /// <exception cref="ArgumentNullException"></exception>
    protected VariableExecutionBase(VariableRef reference, string opCode) : base(opCode)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));
        VariableRef = reference;
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="reference">A reference to the variable to set</param>
    /// <param name="blockId">The id of this block</param>
    /// <param name="opCode">The op code of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    protected VariableExecutionBase(VariableRef reference, string blockId, string opCode) : base(blockId, opCode)
    {
        ArgumentNullException.ThrowIfNull(reference, nameof(reference));
        VariableRef = reference;
    }

    internal VariableExecutionBase(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        VariableRef = new(blockToken, "fields.VARIABLE");
    }

    protected sealed override Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        Variable? variable = context.Variables.FirstOrDefault(var => var.Id.Equals(VariableRef.VarId));
        if (variable is null)
        {
            logger.LogError("Could not find variable with id \"{id}\" and name \"{name}\"", VariableRef.VarId, VariableRef.VarName);
            return Task.CompletedTask;
        }

        return ExecuteInternalAsync(context, variable, logger, ct);
    }

    /// <summary>
    /// The execution of this variable block
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="variable">The variable of this block</param>
    /// <param name="logger">The logger to use</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The async task</returns>
    protected abstract Task ExecuteInternalAsync(ScriptExecutorContext context, Variable variable, ILogger logger, CancellationToken ct = default);
}
