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
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Operator.String;

/// <summary>
/// Get the the char of a specified index
/// </summary>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class CharAt : ValueOperatorBase
{
    public override event Action OnValueChanged
    {
        add
        {
            IndexProvider.OnValueChanged += value;
            StringProvider.OnValueChanged += value;
        }
        remove
        {
            IndexProvider.OnValueChanged -= value;
            StringProvider.OnValueChanged -= value;
        }
    }

    /// <summary>
    /// The provider of the index
    /// </summary>
    public IValueProvider IndexProvider { get; }

    /// <summary>
    /// The provider of the source string
    /// </summary>
    public IValueProvider StringProvider { get; }

    private const string _constOpCode = "operator_letter_of";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public CharAt() : base(_constOpCode)
    {
        IndexProvider = new Empty(DataType.PositiveInteger);
        StringProvider = new Empty(DataType.String);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="index">The index of the char to get</param>
    /// <param name="source">The source string of the char to get</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public CharAt(int index, string source) : this(index, source, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="index">The index of the char to get</param>
    /// <param name="source">The source string of the char to get</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public CharAt(int index, string source, string blockId) : base(blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(index, nameof(index));
        if (index <= 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, "A value larger than 0 was expected.");
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        IndexProvider = new Result(index, true);
        StringProvider = new Result(source);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="indexProvider">The provider of the index of the char to get</param>
    /// <param name="sourceProvider">The provider of the source string of the char to get</param>
    /// <exception cref="ArgumentNullException"></exception>
    public CharAt(IValueProvider indexProvider, IValueProvider sourceProvider) : this(indexProvider, sourceProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="indexProvider">The provider of the index of the char to get</param>
    /// <param name="sourceProvider">The provider of the source string of the char to get</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public CharAt(IValueProvider indexProvider, IValueProvider sourceProvider, string blockId) : base(blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(indexProvider, nameof(indexProvider));
        ArgumentNullException.ThrowIfNull(sourceProvider, nameof(sourceProvider));

        IndexProvider = indexProvider;
        if (IndexProvider is IConstProvider constIndexProvider)
            constIndexProvider.DataType = DataType.PositiveInteger;

        StringProvider = sourceProvider;
        if (StringProvider is IConstProvider constSourceProvider)
            constSourceProvider.DataType = DataType.String;
    }

    internal CharAt(string blockId, JToken blockToken) : base(blockId, _constOpCode)
    {
        IndexProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.LETTER") ?? new Empty(DataType.PositiveInteger);
        StringProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.STRING") ?? new Empty(DataType.String);
    }

    public override async Task<ScratchTypeBase> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double index = (await IndexProvider.GetResultAsync(context, logger, ct)).GetNumberValue();
        string sourceString = (await StringProvider.GetResultAsync(context, logger, ct)).GetStringValue();

        if (index % 1 != 0)
        {
            int rounded = (int)Math.Round(index);
            logger.LogWarning("Unable to get a char by an index that is fractional. In cause of this a rounded index is used. Value: {index}; Rounded: {rounded}", index, rounded);
            index = rounded;
        }

        if (index <= 0 || index > sourceString.Length)     // Return string.empty when the index is out of range
            return new StringType(string.Empty);

        char c = sourceString[(int)index + 1];
        return new StringType(c.ToString());
    }

    private string GetDebuggerDisplay()
    {
        double index = IndexProvider.GetDefaultResult().GetNumberValue();
        string sourceString = StringProvider.GetDefaultResult().GetStringValue();

        return string.Format("{0}th char of \"{1}\"", index, sourceString);
    }
}
