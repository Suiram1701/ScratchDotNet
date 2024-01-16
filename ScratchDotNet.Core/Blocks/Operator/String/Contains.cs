using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;

namespace ScratchDotNet.Core.Blocks.Operator.String;

/// <summary>
/// Indicates whether a string conatins another string
/// </summary>
[OperatorCode(_constOpCode)]
public class Contains : ValueOperatorBase, IBoolValueProvider
{
    public override event Action OnValueChanged
    {
        add
        {
            String1Provider.OnValueChanged += value;
            String2Provider.OnValueChanged += value;
        }
        remove
        {
            String1Provider.OnValueChanged -= value;
            String2Provider.OnValueChanged -= value;
        }
    }

    /// <summary>
    /// The provider of the main string
    /// </summary>
    public IValueProvider String1Provider { get; }

    /// <summary>
    /// The provider of the string that could be contained in the main string
    /// </summary>
    public IValueProvider String2Provider { get; }

    private const string _constOpCode = "operator_contains";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public Contains() : base(_constOpCode)
    {
        String1Provider = new Empty(DataType.String);
        String2Provider = new Empty(DataType.String);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="string1">The main string</param>
    /// <param name="string2">The string that could be conatined in the main string</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Contains(string string1, string string2) : this(string1, string2, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="string1">The main string</param>
    /// <param name="string2">The string that could be conatined in the main string</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Contains(string string1, string string2, string blockId) : base(blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(string1, nameof(string1));
        ArgumentNullException.ThrowIfNull(string2, nameof(string2));

        String1Provider = new Result(string1);
        String2Provider = new Result(string2);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="string1Provider">The provider of the main string</param>
    /// <param name="string2Provider">The provider of the string that could be conatined in the main string</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Contains(IValueProvider string1Provider, IValueProvider string2Provider) : this(string1Provider, string2Provider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="string1Provider">The provider of the main string</param>
    /// <param name="string2Provider">The provider of the string that could be conatined in the main string</param>
    /// <param name="blockId">The id of the this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Contains(IValueProvider string1Provider, IValueProvider string2Provider, string blockId) : base(blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(string1Provider, nameof(string1Provider));
        ArgumentNullException.ThrowIfNull(string2Provider, nameof(string2Provider));

        String1Provider = string1Provider;
        if (String1Provider is IConstProvider const1Provider)
            const1Provider.DataType = DataType.String;

        String2Provider = string2Provider;
        if (String2Provider is IConstProvider const2Provider)
            const2Provider.DataType = DataType.String;
    }

    internal Contains(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        String1Provider = BlockHelpers.GetDataProvider(blockToken, "inputs.STRING1") ?? new Empty(DataType.String);
        String2Provider = BlockHelpers.GetDataProvider(blockToken, "inputs.STRING2") ?? new Empty(DataType.String);
    }

    public override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        string string1 = (await String1Provider.GetResultAsync(context, logger, ct)).GetStringValue();
        string string2 = (await String2Provider.GetResultAsync(context, logger, ct)).GetStringValue();

        bool result = string1.Contains(string2, StringComparison.OrdinalIgnoreCase);
        return new BooleanType(result);
    }
}
