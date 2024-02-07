using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Operator.String;

/// <summary>
/// Joins two strings together
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Join : ValueOperatorBase
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
    /// The provider of the first string to join
    /// </summary>
    public IValueProvider String1Provider { get; }

    /// <summary>
    /// The provider of the seconds string to join
    /// </summary>
    public IValueProvider String2Provider { get; }

    private const string _constOpCode = "operator_join";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public Join() : base(_constOpCode)
    {
        String1Provider = new Empty(DataType.String);
        String2Provider = new Empty(DataType.String);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="string1">The first string to join</param>
    /// <param name="string2">The second string to join</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Join(string string1, string string2) : this(string1, string2, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="string1">The first string to join</param>
    /// <param name="string2">The second string to join</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException">
    public Join(string string1, string string2, string blockId) : base(blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(string1, string2);
        ArgumentNullException.ThrowIfNull(string2, string2);

        String1Provider = new Result(string1);
        String2Provider = new Result(string2);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="string1Provider">The provider of the first string</param>
    /// <param name="string2Provider">The provider of the second string</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Join(IValueProvider string1Provider, IValueProvider string2Provider) : this(string1Provider, string2Provider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="string1Provider">The provider of the first string</param>
    /// <param name="string2Provider">The provider of the second string</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Join(IValueProvider string1Provider, IValueProvider string2Provider, string blockId) : base(blockId, _constOpCode)
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

    internal Join(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        String1Provider = BlockHelpers.GetDataProvider(blockToken, "inputs.STRING1") ?? new Empty(DataType.String);
        String2Provider = BlockHelpers.GetDataProvider(blockToken, "inputs.STRING2") ?? new Empty(DataType.String);
    }

    public override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        string string1 = (await String1Provider.GetResultAsync(context, logger, ct)).GetStringValue();
        string string2 = (await String2Provider.GetResultAsync(context, logger, ct)).GetStringValue();

        string result = string1 + string2;
        return new StringType(result);
    }

    private string GetDebuggerDisplay()
    {
        string string1 = String1Provider.GetDefaultResult().GetStringValue();
        string string2 = String2Provider.GetDefaultResult().GetStringValue();

        return string.Format("Join: {0} + {1}", string1, string2);
    }
}
