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

namespace ScratchDotNet.Core.Blocks.Operator.Math;

/// <summary>
/// Provides some mathematics functions
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Mathop : ValueOperatorBase
{
    public override event EventHandler<ValueChangedEventArgs> OnValueChanged
    {
        add => ValueProvider.OnValueChanged += value;
        remove => ValueProvider.OnValueChanged -= value;
    }

    /// <summary>
    /// The operation to execute
    /// </summary>
    public MathopOperation Operation { get; }

    /// <summary>
    /// The provider of the value to be calculated with
    /// </summary>
    [Input("NUM")]
    public IValueProvider ValueProvider
    {
        get => _valueProvider;
        set
        {
            ThrowAtRuntime();
            _valueProvider = value;
        }
    }
    private IValueProvider _valueProvider;

    private const string _constOpCode = "operator_mathop";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    /// <param name="value">The value to be calculated with</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Mathop(MathopOperation operation, double value) : this(operation, value, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    /// <param name="value">The value to be calculated with</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Mathop(MathopOperation operation, double value, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(operation, nameof(operation));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        Operation = operation;
        _valueProvider = new DoubleValue(value);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    /// <param name="valueProvider">The provider of the value to be calculated with</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Mathop(MathopOperation operation, IValueProvider valueProvider) : this(operation, valueProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    /// <param name="valueProvider">The provider of the value to be calculated with</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Mathop(MathopOperation operation, IValueProvider valueProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(operation, nameof(operation));
        ArgumentNullException.ThrowIfNull(valueProvider, nameof(valueProvider));

        Operation = operation;
        _valueProvider = valueProvider;
    }

#pragma warning disable CS8618
    internal Mathop(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
        string @operator = blockToken.SelectToken("fields.OPERATOR[0]")!.Value<string>()!;
        Operation = EnumNameAttributeHelpers.ParseEnumWithName<MathopOperation>(@operator);
    }

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double value = (await ValueProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        double result = GetOperationDelegate()(value);

        return new DoubleValue(result);
    }

    private Func<double, double> GetOperationDelegate()
    {
        return Operation switch
        {
            MathopOperation.Abs => System.Math.Abs,
            MathopOperation.Floor => System.Math.Floor,
            MathopOperation.Ceiling => System.Math.Ceiling,
            MathopOperation.Sqrt => System.Math.Sqrt,
            MathopOperation.Sin => System.Math.Sin,
            MathopOperation.Cos => System.Math.Cos,
            MathopOperation.Tan => System.Math.Tan,
            MathopOperation.Asin => System.Math.Asin,
            MathopOperation.Acos => System.Math.Acos,
            MathopOperation.Atan => System.Math.Atan,
            MathopOperation.Ln => System.Math.Log,
            MathopOperation.Log => System.Math.Log10,
            MathopOperation.PowE => System.Math.Exp,
            MathopOperation.Pow10 => x => System.Math.Pow(10, x),
            _ => throw new NotSupportedException("The specified mathop operation isn't supported.")
        };
    }

    private string GetDebuggerDisplay()
    {
        double value = ValueProvider.GetDefaultResult().ConvertToDoubleValue();
        string operation = Operation.ToString();

        return string.Format("{0} of {1}", operation, value);
    }
}
