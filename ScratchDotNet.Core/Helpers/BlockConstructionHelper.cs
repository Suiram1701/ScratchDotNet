using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Data;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Helpers;

/// <summary>
/// A helper class that is used to construct a block by the json a doc
/// </summary>
internal class BlockConstructionHelper(BlockBase block)
{
    private readonly BlockBase _block = block;

    /// <summary>
    /// Iterate every property of the type <see cref="IValueProvider"/> that have a <see cref="InputAttribute"/> and set a value provider read from <paramref name="blockToken"/>
    /// </summary>
    /// <remarks>
    /// The <paramref name="blockToken"/> have to be at the root position of the block
    /// </remarks>
    /// <param name="blockToken">The json doc </param>
    public void ConstructInputs(JToken blockToken)
    {
        IEnumerable<PropertyInfo> inputProperties = _block.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(prop => prop.PropertyType.RecursiveTypeBaseTypeSearch(typeof(IValueProvider)))     // Only properties that has the type IValueProvider or inherit from it
            .Where(prop => prop.IsDefined(typeof(InputAttribute)))
            .Where(prop => prop.CanWrite);     // skip getter-only properties
        foreach (PropertyInfo inputProperty in inputProperties)
        {
            InputAttribute inputAttribute = inputProperty.GetCustomAttribute<InputAttribute>()!;

            string inputName = inputAttribute.Name ?? GetNameByMemberName(inputProperty);
            string path = "inputs." + inputName;

            IValueProvider valueProvider;
            if (inputProperty.PropertyType.RecursiveTypeBaseTypeSearch(typeof(IBoolValueProvider)))
                valueProvider = GetBoolInputProviderFromJSON(blockToken, path);
            else
                valueProvider = GetInputProviderFromJSON(blockToken, path);

            inputProperty.SetValue(_block, valueProvider, null);
        }
    }

    /// <summary>
    /// Iterate every property of the type <see cref="Substack"/> that have a <see cref="SubstackAttribute"/> and adds the blocks read from <paramref name="blockToken"/>
    /// </summary>
    /// <remarks>
    /// The <paramref name="blockToken"/> have to be at the root position of the block
    /// </remarks>
    /// <param name="blockToken">The json doc </param>
    public void ConstructSubstacks(JToken blockToken)
    {
        IEnumerable<PropertyInfo> substackProperties = _block.GetType().GetProperties(BindingFlags.Instance| BindingFlags.Public)
            .Where(prop => prop.PropertyType == typeof(Substack))
            .Where(prop => prop.IsDefined(typeof(SubstackAttribute)))
            .Where(prop => prop.CanWrite);     // skip getter-only properties
        foreach (PropertyInfo substackProperty in substackProperties)
        {
            SubstackAttribute substackAttribute = substackProperty.GetCustomAttribute<SubstackAttribute>()!;

            string substackName = substackProperty.Name ?? GetNameByMemberName(substackProperty);
            string path = "inputs." + substackName;

            Substack substack = new(blockToken, path);
            substackProperty.SetValue(_block, substack, null);
        }
    }

    /// <summary>
    /// Reads the data specified at <paramref name="jsonPath"/> and create the provider of the data 
    /// </summary>
    /// <param name="blockToken">The json doc to read the data from</param>
    /// <param name="jsonPath">The path of the data</param>
    /// <returns>The created provider</returns>
    public static IValueProvider GetInputProviderFromJSON(JToken blockToken, string jsonPath)
    {
        JToken? dataToken = blockToken.SelectToken(jsonPath)?[1];

        if (dataToken is null || dataToken.Type == JTokenType.Null)     // value of path was empty
            return new EmptyValue();
        else if (dataToken.Type == JTokenType.Array)     // a constant value
            return GetConstValue(dataToken);
        else if (dataToken.Type == JTokenType.String)     // Reference to another block
            return GetReferenceBlock(blockToken, dataToken.Value<string>()!);
        else
        {
            string json = dataToken.ToString();
            throw new ArgumentException(string.Format("Could not determine data from json '{0}'", json));
        }
    }

    /// <summary>
    /// Calls <see cref="GetInputProviderFromJSON(JToken, string)"/> and throws an exception when the provider read from the doc doesn't implement <see cref="IBoolValueProvider"/>
    /// </summary>
    /// <param name="blockToken">The token of the block that request the provider</param>
    /// <param name="dataPath">The relative JSON path to the data</param>
    /// <returns>The provider</returns>
    public static IBoolValueProvider GetBoolInputProviderFromJSON(JToken blockToken, string dataPath)
    {
        IValueProvider valueProvider = GetInputProviderFromJSON(blockToken, dataPath);

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
    /// <param name="dataToken">The json token of data</param>
    /// <returns>The created provider</returns>
    private static IValueProvider GetConstValue(JToken dataToken)
    {
        DataType dataType = (DataType)dataToken[0]!.Value<int>();
        string? dataValue = dataToken[1]?.Value<string>();

        if (string.IsNullOrEmpty(dataValue))
            return new EmptyValue();

        // creates an provider instance with the data type
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

    private static string GetNameByMemberName(MemberInfo member)
    {
        const string suffix = "Provider";     // This is the default suffix used for value providers

        string name = member.Name;

        // Remove the suffix if available
        int suffixIndex = name.IndexOf(suffix);
        if (suffixIndex != -1)
            name = name.Remove(suffixIndex);

        return name.ToUpper();
    }
}
