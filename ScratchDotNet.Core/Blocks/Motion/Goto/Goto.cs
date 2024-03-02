using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core.Blocks.Attributes;
using ScratchDotNet.Core.Blocks.Bases;
using ScratchDotNet.Core.Blocks.Interfaces;
using ScratchDotNet.Core.Enums;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Execution;
using ScratchDotNet.Core.Extensions;
using ScratchDotNet.Core.StageObjects;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;
using System.Diagnostics;

namespace ScratchDotNet.Core.Blocks.Motion.Goto;

/// <summary>
/// Moves a figure to a specified position
/// </summary>
/// <remarks>
/// This can only executed from a figure
/// </remarks>
[ExecutionBlockCode(_constOpCode)]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Goto : ExecutionBlockBase
{
    /// <summary>
    /// The position to move to
    /// </summary>
    [Input("TO")]
    public IValueProvider TargetProvider
    {
        get => _targetProvider;
        set
        {
            ThrowAtRuntime();
            _targetProvider = value;
        }
    }
    private IValueProvider _targetProvider;

    private const string _constOpCode = "motion_goto";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// The <see cref="SpecialTarget"/> <see cref="SpecialTarget.Random"/> isn't supported by this block
    /// </remarks>
    /// <param name="target">The special target where the figure should go to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(SpecialTarget target) : this(target, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// The <see cref="SpecialTarget"/> <see cref="SpecialTarget.Random"/> isn't supported by this block
    /// </remarks>
    /// <param name="target">The special target where the figure should go to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(SpecialTarget target, string blockId) : this(MotionHelpers.GetTargetString(target), blockId)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        if (target == SpecialTarget.Random)
        {
            string message = string.Format("The {0} {1} isn't supported by this block.", nameof(SpecialTarget), nameof(SpecialTarget.Random));
            throw new ArgumentException(message);
        }

        TargetProvider = new TargetReporter(target.ToString());
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The target figure where the figure should go to</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(IFigure target) : this(target, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="target">The target figure where the figure should go to</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(IFigure target, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(target, nameof(target));
        _targetProvider = new TargetReporter(target.Name);
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// A target provider that implements <see cref="IScratchType"/> is not supported. To provide a constant value you have use a constructor that takes an instance of <see cref="SpecialTarget"/> or <see cref="IFigure"/>
    /// </remarks>
    /// <param name="targetProvider">The provider of the target figure</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(IValueProvider targetProvider) : this(targetProvider, BlockHelpers.GenerateBlockId())
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <remarks>
    /// A target provider that implements <see cref="IScratchType"/> is not supported. To provide a constant value you have use a constructor that takes an instance of <see cref="SpecialTarget"/> or <see cref="IFigure"/>
    /// </remarks>
    /// <param name="targetProvider">The provider of the target figure</param>
    /// <param name="blockId">The id of this block</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public Goto(IValueProvider targetProvider, string blockId) : base(_constOpCode, blockId)
    {
        ArgumentNullException.ThrowIfNull(targetProvider, nameof(targetProvider));

        _targetProvider = targetProvider;
        if (targetProvider is IScratchType)
        {
            string message = string.Format(
                "A target provider that implements {0} is not supported. To provide a constant target you have use a constructor that takes an instance of {1} or {2}",
                nameof(IScratchType),
                nameof(SpecialTarget),
                nameof(IFigure));
            throw new ArgumentException(message, nameof(targetProvider));
        }
    }

#pragma warning disable CS8618
    internal Goto(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
    {
    }

    protected internal override async Task ExecuteAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default)
    {
        if (context.Executor is not IExecutableFigure figure)
        {
            logger.LogWarning("Block {block} have to executed by a figure", BlockId);
            return;
        }

        string target = (await TargetProvider.GetResultAsync(context, logger, ct)).ConvertToStringValue();
        (double x, double y) = MotionHelpers.GetTargetPosition(target, context, logger);

        figure.MoveTo(x, y);
    }

    private string GetDebuggerDisplay()
    {
        string target = TargetProvider.GetDefaultResult().ConvertToStringValue();
        string targetString = target switch
        {
            "_random_" => "random position",
            "_mouse_" => "mouse position",
            _ => string.Format("figure {0}", target)
        };

        return string.Format("Goto: {0}", targetString);
    }

    [OperatorCode(_constOpCode)]
    private class TargetReporter : ValueOperatorBase
    {
        public override event EventHandler<ValueChangedEventArgs> OnValueChanged { add { } remove { } }

        public string Target { get; }

        private const string _constOpCode = "motion_goto_menu";

        public TargetReporter(string target) : base(BlockHelpers.GenerateBlockId(), _constOpCode)
        {
            Target = target;
        }

#pragma warning disable CS8618
        internal TargetReporter(string blockId, JToken blockToken) : base(blockId, blockToken)
#pragma warning restore CS8618
        {
        }

        public override Task<IScratchType> GetResultAsync(ScriptExecutorContext context, ILogger logger, CancellationToken ct = default) =>
            Task.FromResult<IScratchType>(new StringValue(Target));
    }
}
