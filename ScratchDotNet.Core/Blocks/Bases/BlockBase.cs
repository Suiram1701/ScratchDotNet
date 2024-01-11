﻿using Newtonsoft.Json.Linq;
using System.Text;

namespace ScratchDotNet.Core.Blocks.Bases;

/// <summary>
/// The base for every block
/// </summary>
public abstract class BlockBase
{
    /// <summary>
    /// The id of this block
    /// </summary>
    public string BlockId { get; }

    protected readonly string _opCode;

    /// <summary>
    /// Creates a new instance with an automatic generated block id
    /// </summary>
    /// <param name="opCode">The op code of this block</param>
    protected BlockBase(string opCode) : this(GenerateBlockId(), opCode)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The id of this block</param>
    /// <param name="opCode">The op code of this block</param>
    /// <exception cref="ArgumentException"></exception>
    protected BlockBase(string blockId, string opCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(blockId, nameof(blockId));

        BlockId = blockId;
        _opCode = opCode;
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="blockId">The id of this block</param>
    /// <param name="blockToken">The JToken to read the block from</param>
    protected BlockBase(string blockId, JToken blockToken)
    {
        BlockId = blockId;
        _opCode = blockToken["opcode"]!.Value<string>()!;
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
