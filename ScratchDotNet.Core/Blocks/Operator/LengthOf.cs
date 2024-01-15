using Microsoft.Extensions.Logging;
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Operator;

/// <summary>
/// Gets the length of a string
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class LengthOf : ValueOperatorBase
{
    public override event Action OnValueChanged
    {
        add => StringProvider.OnValueChanged += value;
        remove => StringProvider.OnValueChanged -= value;
    }

    /// <summary>
    /// The provider of the string to get the length from
    /// </summary>
    public IValueProvider StringProvider { get; }

    private const string _constOpCode = "operator_length";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public LengthOf() : base(_constOpCode)
    {
        StringProvider = new Empty(DataType.String);
    }

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
        StringProvider = new Result(@string);
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

        StringProvider = stringProvider;
        if (StringProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.String;
    }

    internal LengthOf(string blockId, JToken blockToken) : base(blockId, _constOpCode)
    {
        StringProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.STRING") ?? new Empty(DataType.String);
    }

    public override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        string value = (await StringProvider.GetResultAsync(context, logger, ct)).GetStringValue();
        return new NumberType(value.Length);
    }

    private string GetDebuggerDisplay()
    {
        string value = StringProvider.GetDefaultResult().GetStringValue();
        return string.Format("Length of: {0}", value);
    }
}
