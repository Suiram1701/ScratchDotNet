using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using System.Collections.ObjectModel;

namespace ScratchDotNet.Core.Blocks;

/// <summary>
/// The executor for multiple block run as script
/// </summary>
internal class ScriptExecutor
{
    private ReadOnlyCollection<ExecutionBlockBase> _blocks;

    /// <summary>
    /// Creates a new script executor
    /// </summary>
    public ScriptExecutor()
    {
        _blocks = new ReadOnlyCollection<ExecutionBlockBase>(new List<ExecutionBlockBase>(0));
    }

    /// <summary>
    /// Creates a new script executor from the specified blocks (it starts with the first real action and not with the trigger)
    /// </summary>
    /// <param name="blocks">The object that contains the blocks to read</param>
    /// <param name="startId">The Id of the block to start with</param>
    /// <returns>The created executor</returns>
    public static ScriptExecutor Create(JObject blocks, string startId, ILogger logger)
    {
        List<ExecutionBlockBase> blockList = new();

        string? nextName = startId;
        do
        {
            JToken? block = blocks[nextName];
            if (block is null)
            {
                logger.LogError("Couldn't find next block with name {name}", nextName);
                break;
            }

            logger.LogInformation("Start initializing block {block}", nextName);

            string? opCode = block["opcode"]?.Value<string>();
            if (!string.IsNullOrEmpty(opCode))
            {
                ExecutionBlockBase? blockInstance;
                try
                { blockInstance = ExecutionBlockCodeAttributeHelpers.GetFromOpCode(opCode, nextName, block); }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Block instance of op code {code} of block {block} could not be constructed", opCode, nextName);
                    goto next;
                }

                if (blockInstance is null)
                {
                    logger.LogError("Could not find registered block for op code: {code}; block: {block}", opCode, nextName);
                    goto next;
                }

                blockList.Add(blockInstance);
                logger.LogInformation("Block {block} successfully initialized", nextName);
            }
            else
            {
                logger.LogError("Unable to determine the op code of block {block}", nextName);
            }

            next:
            nextName = block["next"]?.Value<string>();
        }
        while (!string.IsNullOrEmpty(nextName));

        ScriptExecutor executor = new()
        {
            _blocks = new ReadOnlyCollection<ExecutionBlockBase>(blockList)
        };
        return executor;
    }
}
