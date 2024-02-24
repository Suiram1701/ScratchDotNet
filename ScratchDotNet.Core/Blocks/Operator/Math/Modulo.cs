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
/// Provides the modulo operator
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Modulo : ValueOperatorBase
{
    public override event EventHandler<ValueChangedEventArgs> OnValueChanged
    {
        add
        {
            DividendProvider.OnValueChanged += value;
            DivisorProvider.OnValueChanged += value;
        }
        remove
        {
            DividendProvider.OnValueChanged -= value;
            DivisorProvider.OnValueChanged -= value;
        }
    }

    /// <summary>
    /// The provider of the dividend for modulo
    /// </summary>
    [InputProvider("NUM1")]
    public IValueProvider DividendProvider { get; }

    /// <summary>
    /// The provider of the divisor for modulo
    /// </summary>
    [InputProvider("NUM2")]
    public IValueProvider DivisorProvider { get; }

    private const string _constOpCode = "operator_mod";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="num1">The dividend</param>
    /// <param name="num2">The divisor</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Modulo(double num1, double num2) : this(num1, num2, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="num1">The dividend</param>
    /// <param name="num2">The divisor</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Modulo(double num1, double num2, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(num1, nameof(num1));
        ArgumentNullException.ThrowIfNull(num2, nameof(num2));

        DividendProvider = new DoubleValue(num1);
        DivisorProvider = new DoubleValue(num2);
    }
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="num1Provider">The provider of the dividend</param>
    /// <param name="num2Provider">The provider of the divisor</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Modulo(IValueProvider num1Provider, IValueProvider num2Provider) : this(num1Provider, num2Provider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="num1Provider">The provider of the dividend</param>
    /// <param name="num2Provider">The provider of the divisor</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Modulo(IValueProvider num1Provider, IValueProvider num2Provider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(num1Provider, nameof(num1Provider));
        ArgumentNullException.ThrowIfNull(num2Provider, nameof(num2Provider));
        
        DividendProvider = num1Provider;
        DivisorProvider = num2Provider;
    }

#pragma warning disable CS8618
    internal Modulo(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double dividend = (await DividendProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        double divisor = (await DivisorProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();

        return new DoubleValue(dividend % divisor);
    }

    private string GetDebuggerDisplay()
    {
        double dividend = DividendProvider.GetDefaultResult().ConvertToDoubleValue();
        double dividor = DivisorProvider.GetDefaultResult().ConvertToDoubleValue();

        return string.Format("{0} % {1}", dividend, dividor);
    }
}
