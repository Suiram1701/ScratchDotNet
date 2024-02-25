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

namespace ScratchDotNet.Core.Blocks.Operator.String;

/// <summary>
/// Get the the char of a specified index
/// </summary>
/// <remarks>
/// The index of the char to get is 1-based
/// </remarks>
[OperatorCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class CharAt : ValueOperatorBase
{
    public override event EventHandler<ValueChangedEventArgs> OnValueChanged
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
    [Input("LETTER")]
    public IValueProvider IndexProvider
    {
        get => _indexProvider;
        set
        {
            ThrowAtRuntime();
            _indexProvider = value;
        }
    }
    private IValueProvider _indexProvider;

    /// <summary>
    /// The provider of the source string
    /// </summary>
    [Input]
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

    private const string _constOpCode = "operator_letter_of";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="index">The 1-based index of the char to get</param>
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
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, "A value larger than 0 was expected.");
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        _indexProvider = new DoubleValue(index);
        _stringProvider = new StringValue(source);
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
    /// <param name="indexProvider">The provider of the 1-based index of the char to get</param>
    /// <param name="sourceProvider">The provider of the source string of the char to get</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public CharAt(IValueProvider indexProvider, IValueProvider sourceProvider, string blockId) : base(blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(indexProvider, nameof(indexProvider));
        ArgumentNullException.ThrowIfNull(sourceProvider, nameof(sourceProvider));

        _indexProvider = indexProvider;
        _stringProvider = sourceProvider;
    }

#pragma warning disable CS8618
    internal CharAt(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    public override async Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double index = (await IndexProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        string sourceString = (await StringProvider.GetResultAsync(context, logger, ct)).ConvertToStringValue();

        if (index % 1 != 0)     // Check wether the index is fractional
        {
            int rounded = (int)System.Math.Round(index);
            logger.LogTrace("Unable to get a char by an index that is fractional. In cause of this is a rounded index is used. Value: {index}; Rounded: {rounded}", index, rounded);
            index = rounded;
        }

        if (index <= 0 || index > sourceString.Length)     // Return string.empty when the index is out of range
            return new StringValue(string.Empty);

        char c = sourceString[(int)index + 1];     // The index have to get added with 1 because the provided index is 1-based and indexer works with 0-based index
        return new StringValue(c.ToString());
    }

    private string GetDebuggerDisplay()
    {
        double index = IndexProvider.GetDefaultResult().ConvertToDoubleValue();
        string sourceString = StringProvider.GetDefaultResult().ConvertToStringValue();

        return string.Format("{0}th char of \"{1}\"", ++index, sourceString);
    }
}
