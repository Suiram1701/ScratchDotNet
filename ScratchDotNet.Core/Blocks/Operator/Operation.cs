using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Operator;

/// <summary>
/// Supports the default operators +, -, * and /
/// </summary>
[OperatorCode(_constAddOpCode)]
[OperatorCode(_constSubOpCode)]
[OperatorCode(_constMultOpCode)]
[OperatorCode(_constDivOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Operation : ValueOperatorBase
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
    public ValueOperator Operator { get; }

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
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Operation(ValueOperator @operator) : base(GetOpCodeFromOperation(@operator))
    {
        ArgumentNullException.ThrowIfNull(@operator, nameof(@operator));
        if (@operator.HasAnyFlag())
            throw new ArgumentException("A enum instance with more than one flag is not allowed.", nameof(@operator));

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
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Operation(ValueOperator @operator, double num1, double num2) : this(@operator, num1, num2, GenerateBlockId())
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
    public Operation(ValueOperator @operator, double num1, double num2, string blockId) : base(GetOpCodeFromOperation(@operator), blockId)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(@operator));
        if (@operator.HasAnyFlag())
            throw new ArgumentException("A enum instance with more than one flag is not allowed.", nameof(@operator));
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
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Operation(ValueOperator @operator, IValueProvider num1Provider, IValueProvider num2Provider) : this(@operator, num1Provider, num1Provider, GenerateBlockId())
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
    public Operation(ValueOperator @operator, IValueProvider num1Provider, IValueProvider num2Provider, string blockId) : base(blockId, GetOpCodeFromOperation(@operator))
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(@operator));
        if (@operator.HasAnyFlag())
            throw new ArgumentException("A enum instance with more than one flag is not allowed.", nameof(@operator));
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

    internal Operation(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        Operator = _opCode switch
        {
            _constAddOpCode => ValueOperator.Add,
            _constSubOpCode => ValueOperator.Subtract,
            _constMultOpCode => ValueOperator.Multiply,
            _constDivOpCode => ValueOperator.Divide,
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
            ValueOperator.Add => num1 + num2,
            ValueOperator.Subtract => num1 - num2,
            ValueOperator.Multiply => num1 * num2,
            ValueOperator.Divide => num1 / num2,
            _ => throw new NotSupportedException("The specified operator isn't supported.")
        };
        return new NumberType(result);
    }

    private static string GetOpCodeFromOperation(ValueOperator @operator)
    {
        return @operator switch
        {
            ValueOperator.Add => _constAddOpCode,
            ValueOperator.Subtract => _constSubOpCode,
            ValueOperator.Multiply => _constMultOpCode,
            ValueOperator.Divide => _constDivOpCode,
            _ => throw new NotSupportedException("The specified operator isn't supported.")
        };
    }

    private string GetDebuggerDisplay()
    {
        double num1 = Num1Provider.GetDefaultResult().GetNumberValue();
        double num2 = Num2Provider.GetDefaultResult().GetNumberValue();
        string @operator = Operator switch
        {
            ValueOperator.Add => "+",
            ValueOperator.Subtract => "-",
            ValueOperator.Multiply => "*",
            ValueOperator.Divide => "/",
            _ => "unknow"
        };

        return string.Format("{0} {1} {2}", num1, @operator, num2);
    }
}
