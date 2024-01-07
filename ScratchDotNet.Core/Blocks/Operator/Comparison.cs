﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Scratch.Core.Blocks.Attributes;
using Scratch.Core.Blocks.Bases;
using Scratch.Core.Blocks.Interfaces;
using Scratch.Core.Blocks.Operator.ConstProviders;
using Scratch.Core.Enums;
using Scratch.Core.Extensions;
using Scratch.Core.Types;
using Scratch.Core.Types.Bases;
using System.Diagnostics;

namespace Scratch.Core.Blocks.Operator;

/// <summary>
/// Provides the value comparisons >, < and =
/// </summary>
[OperatorCode(_constGreaterThanOpCode)]
[OperatorCode(_constLessThanOpCode)]
[OperatorCode(_constEqualOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Comparison : ValueOperatorBase, IBoolValueProvider
{
    public override event Action OnValueChanged
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
    /// The comparison operation to execute
    /// </summary>
    public ComparisonOperator Operator { get; }

    /// <summary>
    /// The provider of the first operand
    /// </summary>
    public IValueProvider Operand1Provider { get; }

    /// <summary>
    /// The provider of the second operand
    /// </summary>
    public IValueProvider Operand2Provider { get; }

    private const string _constGreaterThanOpCode = "operator_gt";
    private const string _constLessThanOpCode = "operator_lt";
    private const string _constEqualOpCode = "operator_equals";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The comparison operation to execute</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Comparison(ComparisonOperator @operator) : base(GetOpCodeFromOperator(@operator))
    {
        ArgumentNullException.ThrowIfNull(@operator, nameof(@operator));
        if (@operator.HasAnyFlag())
            throw new ArgumentException("A enum instance with more than one flag is not allowed.", nameof(@operator));

        Operand1Provider = new Empty(DataType.String);
        Operand2Provider = new Empty(DataType.String);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The comparison operation to execute</param>
    /// <param name="operand1">The first operand</param>
    /// <param name="operand2">The second operand</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Comparison(ComparisonOperator @operator, ScratchTypeBase operand1, ScratchTypeBase operand2, string blockId) : base(GetOpCodeFromOperator(@operator), blockId)
    {
        ArgumentNullException.ThrowIfNull(@operator, nameof(@operator));
        if (@operator.HasAnyFlag())
            throw new ArgumentException("A enum instance with more than one flag is not allowed.", nameof(@operator));

        Operator = @operator;
        Operand1Provider = new Result(operand1, DataType.String);
        Operand2Provider = new Result(operand2, DataType.String);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The comparison operation to execute</param>
    /// <param name="operand1Provider">The provider of the first operand</param>
    /// <param name="operand2Provider">The provider of the second</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Comparison(ComparisonOperator @operator, IValueProvider operand1Provider, IValueProvider operand2Provider, string blockId) : base(GetOpCodeFromOperator(@operator), blockId)
    {
        ArgumentNullException.ThrowIfNull(@operator, nameof(@operator));
        if (@operator.HasAnyFlag())
            throw new ArgumentException("A enum instance with more than one flag is not allowed.", nameof(@operator));
        ArgumentNullException.ThrowIfNull(operand1Provider, nameof(operand1Provider));
        ArgumentNullException.ThrowIfNull(operand2Provider, nameof(operand2Provider));

        Operator = @operator;
        Operand1Provider = operand1Provider;
        if (Operand1Provider is IConstProvider opr1ConstProvider)
            opr1ConstProvider.DataType = DataType.String;

        Operand2Provider = operand2Provider;
        if (Operand2Provider is IConstProvider opr2ConstProvider)
            opr2ConstProvider.DataType = DataType.String;
    }

    internal Comparison(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        Operator = _opCode switch
        {
            _constGreaterThanOpCode => ComparisonOperator.GreaterThan,
            _constLessThanOpCode => ComparisonOperator.LessThan,
            _constEqualOpCode => ComparisonOperator.Equals,
            _ => throw new NotSupportedException("The specified operation isn't supported.")
        };

        Operand1Provider = BlockHelpers.GetDataProvider(blockToken, "inputs.OPERAND1") ?? new Empty(DataType.String);
        Operand2Provider = BlockHelpers.GetDataProvider(blockToken, "inputs.OPERAND2") ?? new Empty(DataType.String);
    }

    public override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        ScratchTypeBase opr1 = await Operand1Provider.GetResultAsync(context, logger, ct);
        ScratchTypeBase opr2 = await Operand2Provider.GetResultAsync(context, logger, ct);

        bool result = Operator switch
        {
            ComparisonOperator.GreaterThan => opr1.CompareTo(opr2) > 0,
            ComparisonOperator.LessThan => opr1.CompareTo(opr2) < 0,
            ComparisonOperator.Equals => opr1.Equals(opr2),
            _ => throw new NotSupportedException("The specified operation isn't supported.")
        };
        return new BooleanType(result);
    }

    private static string GetOpCodeFromOperator(ComparisonOperator op)
    {
        return op switch
        {
            ComparisonOperator.GreaterThan => _constGreaterThanOpCode,
            ComparisonOperator.LessThan => _constLessThanOpCode,
            ComparisonOperator.Equals => _constEqualOpCode,
            _ => throw new NotSupportedException("The specified operation isn't supported.")
        };
    }

    private string GetDebuggerDisplay()
    {
        double opr1 = Operand1Provider.GetDefaultResult().GetNumberValue();
        double opr2 = Operand2Provider.GetDefaultResult().GetNumberValue();
        string @operator = Operator switch
        {
            ComparisonOperator.GreaterThan => ">",
            ComparisonOperator.LessThan => "<",
            ComparisonOperator.Equals => "==",
            _ => "unknown"
        };

        return string.Format("{0} {1} {2}", opr1, @operator, opr2);
    }
}