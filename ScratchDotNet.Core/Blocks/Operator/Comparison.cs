using Microsoft.Extensions.Logging;
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
/// Provides the value comparisons >, < and =
/// </summary>
[OperatorCode(_constGreaterThanOpCode)]
[OperatorCode(_constLessThanOpCode)]
[OperatorCode(_constEqualOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Comparison : ValueOperatorBase, IBoolValueProvider
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
    /// The comparison operation to execute
    /// </summary>
    public ComparisonOperator Operator { get; }

    /// <summary>
    /// The provider of the first operand
    /// </summary>
    [Input]
    public IValueProvider Operand1Provider { get; }

    /// <summary>
    /// The provider of the second operand
    /// </summary>
    [Input]
    public IValueProvider Operand2Provider { get; }

    private const string _constGreaterThanOpCode = "operator_gt";
    private const string _constLessThanOpCode = "operator_lt";
    private const string _constEqualOpCode = "operator_equals";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The comparison operation to execute</param>
    /// <param name="operand1">The first operand</param>
    /// <param name="operand2">The second operand</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Comparison(ComparisonOperator @operator, IScratchType operand1, IScratchType operand2, string blockId) : base(GetOpCodeFromOperator(@operator), blockId)
    {
        ArgumentNullException.ThrowIfNull(@operator, nameof(@operator));

        Operator = @operator;
        Operand1Provider = operand1.ConvertToStringValue();
        Operand2Provider = operand2.ConvertToStringValue();
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
        ArgumentNullException.ThrowIfNull(operand1Provider, nameof(operand1Provider));
        ArgumentNullException.ThrowIfNull(operand2Provider, nameof(operand2Provider));

        Operator = @operator;
        Operand1Provider = operand1Provider;
        Operand2Provider = operand2Provider;}

#pragma warning disable CS8618
    internal Comparison(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
        Operator = _opCode switch
        {
            _constGreaterThanOpCode => ComparisonOperator.GreaterThan,
            _constLessThanOpCode => ComparisonOperator.LessThan,
            _constEqualOpCode => ComparisonOperator.Equals,
            _ => throw new NotSupportedException("The specified operation isn't supported.")
        };
    }

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        IScratchType opr1 = await Operand1Provider.GetResultAsync(context, logger, ct);
        IScratchType opr2 = await Operand2Provider.GetResultAsync(context, logger, ct);

        bool result = Operator switch
        {
            ComparisonOperator.GreaterThan => opr1.CompareTo(opr2) > 0,
            ComparisonOperator.LessThan => opr1.CompareTo(opr2) < 0,
            ComparisonOperator.Equals => opr1.Equals(opr2),
            _ => throw new NotSupportedException("The specified operation isn't supported.")
        };
        return new BooleanValue(result);
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
        double opr1 = Operand1Provider.GetDefaultResult().ConvertToDoubleValue();
        double opr2 = Operand2Provider.GetDefaultResult().ConvertToDoubleValue();
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
