using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;

namespace ScratchDotNet.Core.Blocks;

internal static class BlockHelpers
{
    /// <summary>
    /// Read a data provider from a block
    /// </summary>
    /// <typeparam name="TValue">The type of the data</typeparam>
    /// <param name="blockToken">The token of the block that request the provider</param>
    /// <param name="dataPath">The relative JSON path to the data</param>
    /// <returns>The result provider. When <see langword="null"/> the result of the data path was empty</returns>
    public static IValueProvider? GetDataProvider(JToken blockToken, string dataPath)
    {
        int selectedDataValue = blockToken.SelectToken(dataPath + "[0]")!.Value<int>();
        JToken? dataToken = blockToken.SelectToken(dataPath + $"[{selectedDataValue}]");

        if (dataToken is null || dataToken.Type == JTokenType.Null)     // Value of path was empty
            return null;
        else if (dataToken.Type == JTokenType.Array)     // Const result
            return GetStaticValue(dataToken);
        else if (dataToken.Type == JTokenType.String)     // Reference to another block
            return GetReferenceBlock(blockToken, dataToken.Value<string>()!);
        else
        {
            string json = dataToken.ToString();
            string message = string.Format("Could not determine data from json {0}", json);
            throw new ArgumentException(message);
        }
    }

    /// <summary>
    /// Read a boolean data provider from a block
    /// </summary>
    /// <param name="blockToken">The token of the block that request the provider</param>
    /// <param name="dataPath">The relative JSON path to the data</param>
    /// <returns>The provider</returns>
    public static IBoolValueProvider GetBoolDataProvider(JToken blockToken, string dataPath)
    {
        IValueProvider? valueProvider = GetDataProvider(blockToken, dataPath);
        if (valueProvider is null)
            return new EmptyBool();
        if (valueProvider is not IBoolValueProvider provider)
        {
            string message = string.Format("A boolean return type was a expected at '{0}'.", dataPath);
            throw new ArgumentException(message, nameof(blockToken));
        }

        return provider;
    }

    /// <summary>
    /// Get a const result
    /// </summary>
    /// <param name="dataToken">The array of the result</param>
    /// <returns>The result</returns>
    private static IValueProvider GetStaticValue(JToken dataToken)
    {
        DataType dataType = (DataType)dataToken.SelectToken("[0]")!.Value<int>();
        string? dataValue = dataToken.SelectToken("[1]")?.Value<string>();
        if (string.IsNullOrEmpty(dataValue))
            return new Empty(dataType);

        ScratchTypeBase result;
        if ((int)dataType >= 4 && (int)dataType <= 7)     // Number values
        {
            double value = double.Parse(dataValue);
            switch (dataType)
            {
                case DataType.PositiveNumber:
                    if (value < 0)
                        throw new ArgumentOutOfRangeException(nameof(dataToken), value, "A value larger or same than 0 was expected.");
                    break;
                case DataType.PositiveInteger:
                    if (value < 0)
                        throw new ArgumentOutOfRangeException(nameof(dataToken), value, "A value larger or same than 0 was expected.");
                    break;
            }

            result = new NumberType(value);
        }
        else if (dataType == DataType.Angle)
        {
            double angle = double.Parse(dataValue);
            while (angle < 0)
                angle += 360;

            result = new NumberType(angle);
        }
        else if (dataType == DataType.Color)
        {
            Color value = ParseColorFromHex(dataValue);
            result = new ColorType(value);
        }
        else if (dataType == DataType.String)
            result = new StringType(dataValue);
        else if (dataType == DataType.Broadcast)
        {
            throw new NotImplementedException();
        }
        else if (dataType == DataType.Variable)
        {
            throw new NotImplementedException();
        }
        else if (dataType == DataType.List)
        {
            throw new NotImplementedException();
        }
        else
        {
            string message = string.Format("The specified type code {0} isn't supported.", dataType);
            throw new NotSupportedException(message);
        }

        return new Result(result, dataType);
    }

    /// <summary>
    /// Get the operator block for a data
    /// </summary>
    /// <typeparam name="TValue">The type of the data</typeparam>
    /// <param name="blockToken">The token of the main block</param>
    /// <param name="blockId">The id of the data block</param>
    /// <returns>The block</returns>
    private static ValueOperatorBase GetReferenceBlock(JToken blockToken, string blockId)
    {
        JToken dataBlock = blockToken.Root![blockId]!;
        string opCode = dataBlock["opcode"]!.Value<string>()!;

        ValueOperatorBase? @operator = OperatorCodeAttributeHelpers.GetFromOpCode(opCode, blockId, dataBlock);
        if (@operator is null)
        {
            string message = string.Format("Could not find registered block for op code {0}", opCode);
            throw new Exception(message);
        }

        return @operator;
    }

    private static Color ParseColorFromHex(string hex)
    {
        hex = hex.TrimStart('#');

        if (hex.Length == 3) // Handle abbreviated format
        {
            hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);
        }

        if (hex.Length == 6 || hex.Length == 8)
        {
            int startIndex = hex.Length == 8 ? 2 : 0;

            return Color.FromArgb(
                hex.Length == 8 ? int.Parse(hex[..2], NumberStyles.HexNumber) : 255,
                int.Parse(hex.Substring(startIndex, 2), NumberStyles.HexNumber),
                int.Parse(hex.Substring(startIndex + 2, 2), NumberStyles.HexNumber),
                int.Parse(hex.Substring(startIndex + 4, 2), NumberStyles.HexNumber)
            );
        }

        throw new ArgumentException("Invalid HEX color format. Supported formats: RGB (AABBCC) or ARGB (AARRGGBB) or abbreviated (ABC).", nameof(hex));
    }

    /// <summary>
    /// Reads all blocks for a substack with a specified entry point
    /// </summary>
    /// <param name="blockToken">The root object of the blockToken</param>
    /// <param name="jsonPath">The path of the substack entry point id</param>
    /// <returns>The created substack</returns>
    public static ReadOnlyCollection<ExecutionBlockBase> GetSubstack(JToken blockToken, string jsonPath)
    {
        string? startId = blockToken.SelectToken(jsonPath + "[1]")?.Value<string>();
        if (string.IsNullOrEmpty(startId))
            return new(new List<ExecutionBlockBase>(0));

        List<ExecutionBlockBase> substack = new();

        string? nextId = startId;
        do
        {
            JToken? block = blockToken.Root[nextId];
            if (block is null)
                break;

            string? opCode = block["opcode"]?.Value<string>();
            if (!string.IsNullOrEmpty(opCode))
            {
                ExecutionBlockBase? blockInstance = ExecutionBlockCodeAttributeHelpers.GetFromOpCode(opCode, nextId, block);

                if (blockInstance is null)
                {
                    string message = string.Format("Could not find registered block for op code: {0}; block: {1}", opCode, nextId);
                    throw new Exception(message);
                }

                substack.Add(blockInstance);
            }
            else
            {
                string message = string.Format("Unable to determine the op code of block {0}", nextId);
                throw new Exception(message);
            }

            nextId = block["next"]?.Value<string>();
        }
        while (!string.IsNullOrEmpty(nextId));

        return new(substack);
    }

    /// <summary>
    /// Invokes a substack
    /// </summary>
    /// <param name="substack">The substack</param>
    /// <param name="context">The context</param>
    /// <param name="logger">The logger</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>The task</returns>
    public static async Task InvokeSubstackAsync(IEnumerable<ExecutionBlockBase> substack, ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        IDisposable? loggerScope = logger.BeginScope("Start executing substack");

        foreach (ExecutionBlockBase block in substack)
            await block.ExecuteAsync(context, logger, ct);

        loggerScope?.Dispose();
    }
}
