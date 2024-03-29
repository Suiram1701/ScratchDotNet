﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Operator;

/// <summary>
/// Provides the logical operations AND and OR
/// </summary>
[OperatorCode(_constAndOpCode)]
[OperatorCode(_constOrOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Logical : ValueOperatorBase, IBoolValueProvider
{
    public override event EventHandler<ValueChangedEventArgs> OnValueChanged
    {
        add
        {
            Operand1Provider.OnValueChanged += value;
            Operand2Provider.OnValueChanged += value;
        }
        remove
        {
            Operand1Provider.OnValueChanged -= value;
            Operand2Provider.OnValueChanged -= value;
        }
    }

    /// <summary>
    /// The logical operator to execute
    /// </summary>
    public LogicalOperation Operator { get; }

    /// <summary>
    /// The provider of the first operand
    /// </summary>
    [Input]
    public IBoolValueProvider Operand1Provider
    {
        get => _operand1Provider;
        set
        {
            ThrowAtRuntime();
            _operand1Provider = value;
        }
    }
    private IBoolValueProvider _operand1Provider;

    /// <summary>
    /// The provider of the second operand
    /// </summary>
    [Input]
    public IBoolValueProvider Operand2Provider
    {
        get => _operand2Provider;
        set
        {
            ThrowAtRuntime();
            _operand2Provider = value;
        }
    }
    private IBoolValueProvider _operand2Provider;

    private const string _constAndOpCode = "operator_and";
    private const string _constOrOpCode = "operator_or";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The operator to use</param>
    /// <param name="operand1Provider">The provider of the first operand</param>
    /// <param name="operand2Provider">The provider of the first operand</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Logical(LogicalOperation @operator, IBoolValueProvider operand1Provider, IBoolValueProvider operand2Provider) : this(@operator, operand1Provider, operand2Provider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The logical operator to execute</param>
    /// <param name="operand1Provider">The provider of the first operand</param>
    /// <param name="operand2Provider">The provider of the second operand</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Logical(LogicalOperation @operator, IBoolValueProvider operand1Provider, IBoolValueProvider operand2Provider, string blockId) : base(GetOpCodeFromOperator(@operator), blockId)
    {
        ArgumentNullException.ThrowIfNull(@operator, nameof(@operator));
        ArgumentNullException.ThrowIfNull(operand1Provider, nameof(operand1Provider));
        ArgumentNullException.ThrowIfNull(operand2Provider, nameof(operand2Provider));

        Operator = @operator;
        _operand1Provider = operand1Provider;
        _operand2Provider = operand2Provider;
    }

#pragma warning disable CS8618
    internal Logical(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
        Operator = _opCode switch
        {
            _constAndOpCode => LogicalOperation.AND,
            _constOrOpCode => LogicalOperation.OR,
            _ => throw new NotSupportedException("The specified operation ins't supported.")
        };
    }

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        bool opr1 = await Operand1Provider.GetBooleanResultAsync(context, logger, ct);
        bool opr2 = await Operand2Provider.GetBooleanResultAsync(context, logger, ct);

        bool result = Operator switch
        {
            LogicalOperation.AND => opr1 && opr2,
            LogicalOperation.OR => opr1 || opr2,
            _ => throw new NotSupportedException("The specified operator isn't supported")
        };
        return new BooleanValue(result);
    }

    private static string GetOpCodeFromOperator(LogicalOperation op)
    {
        return op switch
        {
            LogicalOperation.AND => _constAndOpCode,
            LogicalOperation.OR => _constOrOpCode,
            _ => throw new NotSupportedException("The specified operation isn't supported.")
        };
    }

    private string GetDebuggerDisplay()
    {
        bool opr1 = Operand1Provider?.GetDefaultResult() as BooleanValue? ?? false;
        bool opr2 = Operand2Provider?.GetDefaultResult() as BooleanValue? ?? false;
        string @operator = Operator switch
        {
            LogicalOperation.AND => "&&",
            LogicalOperation.OR => "||",
            _ => "unknown"
        };

        return string.Format("{0} {1} {2}", opr1, @operator, opr2);
    }
}
