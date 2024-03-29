﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.Types;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Sound.Effect;

/// <summary>
/// Changes 
/// </summary>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ChangeSoundEffectBy : ExecutionBlockBase
{
    /// <summary>
    /// The sound effect that should be changed
    /// </summary>
    public SoundEffect Effect { get; }

    /// <summary>
    /// The provider of the value the sound effect should be set to
    /// </summary>
    [Input]
    public IValueProvider ValueProvider
    {
        get => _valueProvider;
        set
        {
            ThrowAtRuntime();
            _valueProvider = value;
        }
    }
    private IValueProvider _valueProvider;

    private const string _constOpCode = "sound_changeeffectby";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="effect">The sound effect to modify</param>
    /// <param name="value">The value to change the effect by</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangeSoundEffectBy(SoundEffect effect, double value) : this(effect, value, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="effect">The sound effect to modify</param>
    /// <param name="value">The value to change the effect by</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangeSoundEffectBy(SoundEffect effect, double value, string blockId) : base(blockId, _constOpCode)
    {
        ArgumentNullException.ThrowIfNull(effect, nameof(effect));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        Effect = effect;
        _valueProvider = new DoubleValue(value);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="effect">The sound effect to modify</param>
    /// <param name="valueProvider">The provider of the value to change the effect by</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangeSoundEffectBy(SoundEffect effect, IValueProvider valueProvider) : this(effect, valueProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="effect">The sound effect to modify</param>
    /// <param name="valueProvider">The provider of the value to change the effect by</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangeSoundEffectBy(SoundEffect effect, IValueProvider valueProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(effect, nameof(effect));
        ArgumentNullException.ThrowIfNull(valueProvider, nameof(valueProvider));

        Effect = effect;
        _valueProvider = valueProvider;
    }

#pragma warning disable CS8618
    internal ChangeSoundEffectBy(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
        string effectString = blockToken.SelectToken("fields.EFFECT[0]")!.Value<string>()!;
        Effect = EnumNameAttributeHelpers.ParseEnumWithName<SoundEffect>(effectString);
    }

    protected internal override async Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double value = (await ValueProvider.GetResultAsync(context, logger, ct)).ConvertToDoubleValue();
        string effectKey = $"{context.Executor.Name}_{Effect}";

        context.RuntimeData.TryGetValue(effectKey, out object? effectObj);
        double effectValue = effectObj as double? ?? 0.0d;

        effectValue += value;
        context.RuntimeData[effectKey] = effectValue;
    }

    private string GetDebuggerDisplay()
    {
        string effect = Effect.ToString();
        double value = ValueProvider.GetDefaultResult().ConvertToDoubleValue();

        return string.Format("Change sound effect {0} by {1}", effect, value);
    }
}
