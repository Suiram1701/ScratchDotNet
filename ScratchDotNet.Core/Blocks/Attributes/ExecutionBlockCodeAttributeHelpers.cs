using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Extensions;
using System.Reflection;

namespace ScratchDotNet.Core.Blocks.Attributes;

internal static class ExecutionBlockCodeAttributeHelpers
{
    private static readonly Dictionary<string, ConstructorInfo> _mappedOpCodes;

    static ExecutionBlockCodeAttributeHelpers()
    {
        _mappedOpCodes = new();

        Assembly assembly = Assembly.GetAssembly(typeof(ExecutionBlockCodeAttribute))!;
        foreach (Type type in assembly.GetTypes()
            .Cast<Type>()
            .Where(t => t.GetCustomAttributes<ExecutionBlockCodeAttribute>().Any()))
        {
            if (!type.RecursiveTypeBaseTypeSearch(typeof(ExecutionBlockBase)))
            {
                string message = string.Format("Every type that have the {0} have to inherit from {1}. The type {2} doesn't inherit from {1}.",
                    nameof(ExecutionBlockCodeAttribute),
                    nameof(ExecutionBlockBase),
                    type.Name);
                throw new Exception(message);
            }

            ConstructorInfo? constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, new[] { typeof(string), typeof(JToken) });
            if (constructor is null)
                continue;

            foreach (string opCode in type.GetCustomAttributes<ExecutionBlockCodeAttribute>().Select(attr => attr.Code))
            {
                if (_mappedOpCodes.ContainsKey(opCode))
                {
                    string message = string.Format("Could not register multiple types for the op code '{0}'", opCode);
                    throw new Exception(message);
                }
                _mappedOpCodes.Add(opCode, constructor);
            }
        }
    }

    /// <summary>
    /// Creates a instance of the block with the specified op code
    /// </summary>
    /// <param name="opcode">The op code</param>
    /// <param name="blockId">The id of the block</param>
    /// <param name="block">The json data to create the block from</param>
    /// <returns>The created instance. <see langword="null"/> when no type was found</returns>
    public static ExecutionBlockBase? GetFromOpCode(string opcode, string blockId, JToken block)
    {
        if (!_mappedOpCodes.TryGetValue(opcode, out ConstructorInfo? constructor))
            return null;
        return constructor.Invoke(new[] { blockId, (object)block }) as ExecutionBlockBase;
    }
}