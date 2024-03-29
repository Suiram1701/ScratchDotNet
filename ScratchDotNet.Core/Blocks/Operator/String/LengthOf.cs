﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Operator.String;

/// <summary>
/// Gets the length of a string
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class LengthOf : ValueOperatorBase
{
    public override event EventHandler<ValueChangedEventArgs> OnValueChanged
    {
        add => StringProvider.OnValueChanged += value;
        remove => StringProvider.OnValueChanged -= value;
    }

    /// <summary>
    /// The provider of the string to get the length from
    /// </summary>
    public IValueProvider StringProvider
    {
        get => _stringProvider;
        set
        {
            ThrowAtRuntime();
            _stringProvider = value;
        }
    }
    private IValueProvider _stringProvider;

    private const string _constOpCode = "operator_length";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="string">The string to get the length from</param>
    /// <exception cref="ArgumentNullException"></exception>
    public LengthOf(string @string) : this(@string, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="string">The string to get the length from</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public LengthOf(string @string, string blockId) : base(blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(@string, nameof(@string));
        _stringProvider = new StringValue(@string);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="stringProvider">The provider of the string to get the length from</param>
    /// <exception cref="ArgumentNullException"></exception>
    public LengthOf(IValueProvider stringProvider) : this(stringProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="stringProvider">The provider of the string to get the length from</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public LengthOf(IValueProvider stringProvider, string blockId) : base(blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(stringProvider, nameof(@stringProvider));
        _stringProvider = stringProvider;
    }

#pragma warning disable CS8618
    internal LengthOf(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        string value = (await StringProvider.GetResultAsync(context, logger, ct)).ConvertToStringValue();
        return new DoubleValue(value.Length);
    }

    private string GetDebuggerDisplay()
    {
        string value = StringProvider.GetDefaultResult().ConvertToStringValue();
        return string.Format("Length of: {0}", value);
    }
}
