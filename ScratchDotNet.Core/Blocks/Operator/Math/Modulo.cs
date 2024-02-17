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
    /// The provider of the num1Provider
    /// </summary>
    public IValueProvider Num1Provider { get; }

    /// <summary>
    /// The provider of the num2Provider
    /// </summary>
    public IValueProvider Num2Provider { get; }

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

        Num1Provider = new DoubleValue(num1);
        Num2Provider = new DoubleValue(num2);

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
        
        Num1Provider = num1Provider;
        Num2Provider = num2Provider;
    }

    internal Modulo(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        Num1Provider = BlockHelpers.GetDataProvider(blockToken, "inputs.NUM1");
        Num2Provider = BlockHelpers.GetDataProvider(blockToken, "inputs.NUM2");
    }

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double dividend = (await Num1Provider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        double divisor = (await Num2Provider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();

        return new DoubleValue(dividend % divisor);
    }

    private string GetDebuggerDisplay()
    {
        double dividend = Num1Provider.GetDefaultResult().ConvertToDoubleValue();
        double dividor = Num2Provider.GetDefaultResult().ConvertToDoubleValue();

        return string.Format("{0} % {1}", dividend, dividor);
    }
}
