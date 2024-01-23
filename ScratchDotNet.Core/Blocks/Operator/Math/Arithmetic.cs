﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Operator.Math;

/// <summary>
/// Supports the default operators +, -, * and /
/// </summary>
[OperatorCode(_constAddOpCode)]
[OperatorCode(_constSubOpCode)]
[OperatorCode(_constMultOpCode)]
[OperatorCode(_constDivOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Arithmetic : ValueOperatorBase
{
    public override event Action OnValueChanged
    {
        add
        {
            Num1Provider.OnValueChanged += value;
            Num2Provider.OnValueChanged += value;
        }
        remove
        {
            Num1Provider.OnValueChanged -= value;
            Num2Provider.OnValueChanged -= value;
        }
    }

    /// <summary>
    /// The operator to use
    /// </summary>
    public ArithmeticOperator Operator { get; }

    /// <summary>
    /// The provider of the first number
    /// </summary>
    public IValueProvider Num1Provider { get; }

    /// <summary>
    /// The provider of the second number
    /// </summary>
    public IValueProvider Num2Provider { get; }

    private const string _constAddOpCode = "operator_add";
    private const string _constSubOpCode = "operator_subtract";
    private const string _constMultOpCode = "operator_multiply";
    private const string _constDivOpCode = "operator_divide";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The operator to use</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Arithmetic(ArithmeticOperator @operator) : base(GetOpCodeFromOperation(@operator))
    {
        ArgumentNullException.ThrowIfNull(@operator, nameof(@operator));

        Operator = @operator;
        Num1Provider = new Empty(DataType.Number);
        Num2Provider = new Empty(DataType.Number);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The operator to use</param>
    /// <param name="num1">The first number</param>
    /// <param name="num2">The second number</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Arithmetic(ArithmeticOperator @operator, double num1, double num2) : this(@operator, num1, num2, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The operator to use</param>
    /// <param name="num1">The first number</param>
    /// <param name="num2">The second number</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Arithmetic(ArithmeticOperator @operator, double num1, double num2, string blockId) : base(GetOpCodeFromOperation(@operator), blockId)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(@operator));
        ArgumentNullException.ThrowIfNull(num1, nameof(num1));
        ArgumentNullException.ThrowIfNull(num2, nameof(num2));

        Operator = @operator;
        Num1Provider = new Result(num1, false);
        Num2Provider = new Result(num2, false);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The operator to use</param>
    /// <param name="num1Provider">The provider of the first number</param>
    /// <param name="num2Provider">The provider of the second number</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Arithmetic(ArithmeticOperator @operator, IValueProvider num1Provider, IValueProvider num2Provider) : this(@operator, num1Provider, num2Provider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The operator to use</param>
    /// <param name="num1Provider">The provider of the first number</param>
    /// <param name="num2Provider">The provider of the second number</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Arithmetic(ArithmeticOperator @operator, IValueProvider num1Provider, IValueProvider num2Provider, string blockId) : base(blockId, GetOpCodeFromOperation(@operator))
    {
        ArgumentNullException.ThrowIfNull(@operator, nameof(@operator));
        ArgumentNullException.ThrowIfNull(num1Provider, nameof(num1Provider));
        ArgumentNullException.ThrowIfNull(num2Provider, nameof(num2Provider));

        Operator = @operator;
        Num1Provider = num1Provider;
        if (Num1Provider is IConstProvider num1ConstProvider)
            num1ConstProvider.DataType = DataType.Number;

        Num2Provider = num2Provider;
        if (Num2Provider is IConstProvider num2ConstProvider)
            num2ConstProvider.DataType = DataType.Number;
    }

    internal Arithmetic(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        Operator = _opCode switch
        {
            _constAddOpCode => ArithmeticOperator.Add,
            _constSubOpCode => ArithmeticOperator.Subtract,
            _constMultOpCode => ArithmeticOperator.Multiply,
            _constDivOpCode => ArithmeticOperator.Divide,
            _ => throw new NotSupportedException("The specified operator isn't supported.")
        };

        Num1Provider = BlockHelpers.GetDataProvider(blockToken, "inputs.NUM1")
            ?? new Empty(DataType.Number);
        Num2Provider = BlockHelpers.GetDataProvider(blockToken, "inputs.NUM2")
            ?? new Empty(DataType.Number);
    }

    public override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double num1 = (await Num1Provider.GetResultAsync(context, logger, ct)).GetNumberValue();
        double num2 = (await Num2Provider.GetResultAsync(context, logger, ct)).GetNumberValue();

        double result = Operator switch
        {
            ArithmeticOperator.Add => num1 + num2,
            ArithmeticOperator.Subtract => num1 - num2,
            ArithmeticOperator.Multiply => num1 * num2,
            ArithmeticOperator.Divide => num1 / num2,
            _ => throw new NotSupportedException("The specified operator isn't supported.")
        };
        return new NumberType(result);
    }

    private static string GetOpCodeFromOperation(ArithmeticOperator @operator)
    {
        return @operator switch
        {
            ArithmeticOperator.Add => _constAddOpCode,
            ArithmeticOperator.Subtract => _constSubOpCode,
            ArithmeticOperator.Multiply => _constMultOpCode,
            ArithmeticOperator.Divide => _constDivOpCode,
            _ => throw new NotSupportedException("The specified operator isn't supported.")
        };
    }

    private string GetDebuggerDisplay()
    {
        double num1 = Num1Provider.GetDefaultResult().GetNumberValue();
        double num2 = Num2Provider.GetDefaultResult().GetNumberValue();
        string @operator = Operator switch
        {
            ArithmeticOperator.Add => "+",
            ArithmeticOperator.Subtract => "-",
            ArithmeticOperator.Multiply => "*",
            ArithmeticOperator.Divide => "/",
            _ => "unknow"
        };

        return string.Format("{0} {1} {2}", num1, @operator, num2);
    }
}