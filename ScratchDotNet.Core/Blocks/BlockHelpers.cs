using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Data;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Converters;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace ScratchDotNet.Core.Blocks;

/// <summary>
/// Helper methods for blocks
/// </summary>
public static class BlockHelpers
{
    /// <summary>
    /// Read a data provider from a block
    /// </summary>
    /// <typeparam name="TValue">The type of the data</typeparam>
    /// <param name="blockToken">The token of the block that request the provider</param>
    /// <param name="dataPath">The relative JSON path to the data</param>
    /// <returns>The result provider. When <see langword="null"/> the result of the data path was empty</returns>
    internal static IValueProvider GetDataProvider(JToken blockToken, string dataPath)
    {
        JToken? dataToken = blockToken.SelectToken(dataPath + $"[1]");

        if (dataToken is null || dataToken.Type == JTokenType.Null)     // Value of path was empty
            return new EmptyValue();
        else if (dataToken.Type == JTokenType.Array)     // a constant value
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
    /// Reads a boolean value provider from a block
    /// </summary>
    /// <param name="blockToken">The token of the block that request the provider</param>
    /// <param name="dataPath">The relative JSON path to the data</param>
    /// <returns>The provider</returns>
    internal static IBoolValueProvider GetBoolDataProvider(JToken blockToken, string dataPath)
    {
        IValueProvider valueProvider = GetDataProvider(blockToken, dataPath);
        if (valueProvider is EmptyValue)
            return new EmptyBoolValue();

        if (valueProvider is not IBoolValueProvider provider)
        {
            string message = string.Format("A boolean value provider was a expected at '{0}'.", dataPath);
            throw new ArgumentException(message, nameof(blockToken));
        }

        return provider;
    }

    /// <summary>
    /// Reads a constant value from an input
    /// </summary>
    /// <param name="dataToken">The token of the provider</param>
    /// <returns>The result</returns>
    private static IValueProvider GetStaticValue(JToken dataToken)
    {
        DataType dataType = (DataType)dataToken[0]!.Value<int>();
        string? dataValue = dataToken[1]?.Value<string>();

        if (string.IsNullOrEmpty(dataValue))
            return new EmptyValue();

        switch (dataType)
        {
            case DataType.Number:
            case DataType.PositiveNumber:
            case DataType.Integer:
            case DataType.PositiveInteger:
                DoubleValue doubleValue = DoubleValue.Parse(dataValue ?? string.Empty, null);
                ThrowWhenNotDataType(doubleValue, dataType);

                return doubleValue;
            case DataType.Angle:
                double angleDouble = double.Parse(dataValue, null);
                AngleConverter converter = new(angleDouble);

                return new DoubleValue(converter.ConvertToNormalFormat());
            case DataType.String:
                return new StringValue(dataValue);
            case DataType.Variable:
                string varName = dataValue!;
                string varId = dataToken[2]!.Value<string>()!;

                return new VariableContent(new(varName, varId));
            case DataType.List:
                string listName = dataValue!;
                string listId = dataToken[2]!.Value<string>()!;

                return new ListContent(new(listName, listId));
            default:
                string message = string.Format("The DataType {0} isn't implemented.", dataType);
                throw new NotImplementedException(message);
        }
    }

    /// <summary>
    /// Reads a reference block of an input value
    /// </summary>
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

    /// <summary>
    /// Throws an exception when type <paramref name="type"/> not accept <paramref name="value"/>
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="type">The type with which <paramref name="value"/> should be validated</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static void ThrowWhenNotDataType(DoubleValue value, DataType type)
    {
        if (type == DataType.Integer || type == DataType.PositiveInteger)
        {
            if (value.Value < 0)
                throw new ArgumentOutOfRangeException(null, value.Value, "A value of the type integer mustn't be a fractional number.");
        }

        if (type == DataType.PositiveNumber || type == DataType.PositiveInteger)
        {
            if (value.Value < 0)
                throw new ArgumentOutOfRangeException(null, value.Value, "A positive value have to be larger or same than 0.");
        }
    }

    /// <summary>
    /// Reads all blocks for a substack with a specified entry point
    /// </summary>
    /// <param name="blockToken">The root object of the blockToken</param>
    /// <param name="jsonPath">The path of the substack entry point id</param>
    /// <returns>The created substack</returns>
    internal static ReadOnlyCollection<ExecutionBlockBase> GetSubstack(JToken blockToken, string jsonPath)
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
    internal static async Task InvokeSubstackAsync(IEnumerable<ExecutionBlockBase> substack, ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        IDisposable? loggerScope = logger.BeginScope("Start executing substack");

        foreach (ExecutionBlockBase block in substack)
            await block.ExecuteAsync(context, logger, ct);

        loggerScope?.Dispose();
    }

    /// <summary>
    /// Generates a random block id
    /// </summary>
    /// <returns>The generated id</returns>
    public static string GenerateBlockId()
    {
        const string characters = "#abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{}|;:'<>,.?/";

        StringBuilder randomString = new(20);
        for (int i = 0; i < 20; i++)
        {
            int randomIndex = Random.Shared.Next(characters.Length);
            randomString.Append(characters[randomIndex]);
        }

        return randomString.ToString();
    }
}
