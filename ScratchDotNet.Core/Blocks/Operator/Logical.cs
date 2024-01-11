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
/// Provides the logical operations AND and OR
/// </summary>
[OperatorCode(_constAndOpCode)]
[OperatorCode(_constOrOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Logical : ValueOperatorBase, IBoolValueProvider
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
    /// The logical operator to execute
    /// </summary>
    public LogicalOperation Operator { get; }

    /// <summary>
    /// The provider of the first operand
    /// </summary>
    public IBoolValueProvider Operand1Provider { get; }

    /// <summary>
    /// The provider of the second operand
    /// </summary>
    public IBoolValueProvider Operand2Provider { get; }

    private const string _constAndOpCode = "operator_and";
    private const string _constOrOpCode = "operator_or";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The operator to use</param>
    public Logical(LogicalOperation @operator) : base(GetOpCodeFromOperator(@operator))
    {
        ArgumentNullException.ThrowIfNull(@operator, nameof(@operator));
        
        Operator = @operator;
        Operand1Provider = new EmptyBool();
        Operand2Provider = new EmptyBool();
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operator">The operator to use</param>
    /// <param name="operand1Provider">The provider of the first operand</param>
    /// <param name="operand2Provider">The provider of the first operand</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Logical(LogicalOperation @operator, IBoolValueProvider operand1Provider, IBoolValueProvider operand2Provider) : this(@operator, operand1Provider, operand2Provider, GenerateBlockId())
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
        Operand1Provider = operand1Provider;
        Operand2Provider = operand2Provider;
    }

    internal Logical(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        Operator = _opCode switch
        {
            _constAndOpCode => LogicalOperation.AND,
            _constOrOpCode => LogicalOperation.OR,
            _ => throw new NotSupportedException("The specified operation ins't supported.")
        };

        Operand1Provider = BlockHelpers.GetBoolDataProvider(blockToken, "inputs.OPERAND1");
        Operand2Provider = BlockHelpers.GetBoolDataProvider(blockToken, "inputs.OPERAND2");
    }

    public override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        bool opr1 = (await Operand1Provider.GetResultAsync(context, logger, ct)).GetBoolValue();
        bool opr2 = (await Operand2Provider.GetResultAsync(context, logger, ct)).GetBoolValue();

        bool result = Operator switch
        {
            LogicalOperation.AND => opr1 && opr2,
            LogicalOperation.OR => opr1 || opr2,
            _ => throw new NotSupportedException("The specified operator isn't supported")
        };
        return new BooleanType(result);
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
        bool opr1 = Operand1Provider?.GetDefaultResult().GetBoolValue() ?? false;
        bool opr2 = Operand2Provider?.GetDefaultResult().GetBoolValue() ?? false;
        string @operator = Operator switch
        {
            LogicalOperation.AND => "&&",
            LogicalOperation.OR => "||",
            _ => "unknown"
        };

        return string.Format("{0} {1} {2}", opr1, @operator, opr2);
    }
}
