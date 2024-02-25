using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;

namespace ScratchDotNet.Core.Blocks.Operator.String;

/// <summary>
/// Indicates whether a string conatins another string
/// </summary>
[OperatorCode(_constOpCode)]
public class Contains : ValueOperatorBase, IBoolValueProvider
{
    public override event EventHandler<ValueChangedEventArgs> OnValueChanged
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
    [Input]
    public IValueProvider String1Provider
    {
        get => _string1Provider;
        set
        {
            ThrowAtRuntime();
            _string1Provider = value;
        }
    }
    private IValueProvider _string1Provider;

    /// <summary>
    /// The provider of the string that could be contained in the main string
    /// </summary>
    [Input]
    public IValueProvider String2Provider
    {
        get => _string2Provider;
        set
        {
            ThrowAtRuntime();
            _string2Provider = value;
        }
    }
    private IValueProvider _string2Provider;

    private const string _constOpCode = "operator_contains";

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

        _string1Provider = new StringValue(string1);
        _string2Provider = new StringValue(string2);
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

        _string1Provider = string1Provider;
        _string2Provider = string2Provider;
    }

#pragma warning disable CS8618
    internal Contains(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        string string1 = (await String1Provider.GetResultAsync(context, logger, ct)).ConvertToStringValue();
        string string2 = (await String2Provider.GetResultAsync(context, logger, ct)).ConvertToStringValue();

        bool result = string1.Contains(string2, StringComparison.OrdinalIgnoreCase);
        return new BooleanValue(result);
    }
}
