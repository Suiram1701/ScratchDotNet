using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Blocks.Operator.ConstProviders;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public IValueProvider ValueProvider { get; }

    private const string _constOpCode = "sound_changeeffectby";

    /// <summary>
    /// Creates a new inszance
    /// </summary>
    /// <param name="effect">The sound effect to modify</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangeSoundEffectBy(SoundEffect effect) : this(effect, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="effect">The sound effect to modify</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ChangeSoundEffectBy(SoundEffect effect, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(effect, nameof(effect));
        Effect = effect;

        ValueProvider = new Result(0, false);
    }

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
        ValueProvider = new Result(value, false);
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

        ValueProvider = valueProvider;
        if (ValueProvider is IConstProvider constProvider)
            constProvider.DataType = DataType.Number;
    }

    internal ChangeSoundEffectBy(string blockId, JToken blockToken) : base(blockId, blockToken)
    {
        string effectString = blockToken.SelectToken("fields.EFFECT[0]")!.Value<string>()!;
        Effect = EnumNameAttributeHelpers.ParseEnumWithName<SoundEffect>(effectString);

        ValueProvider = BlockHelpers.GetDataProvider(blockToken, "inputs.VALUE") ?? new Empty(DataType.Number);
    }

    protected override async Task ExecuteInternalAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        double value = (await ValueProvider.GetResultAsync(context, logger, ct)).GetNumberValue();
        switch (Effect)
        {
            case SoundEffect.Pitch:
                context.Executor.SoundPitch += value;
                break;
            case SoundEffect.Panorama:
                context.Executor.SoundPanorama += value;
                break;
        }
    }

    private string GetDebuggerDisplay()
    {
        string effect = Effect.ToString();
        double value = ValueProvider.GetDefaultResult().GetNumberValue();

        return string.Format("Change sound effect {0} by {1}", effect, value);
    }
}
