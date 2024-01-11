namespace ScratchDotNet.Core.Blocks.Attributes;

/// <summary>
/// Set the op code of a block
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class ExecutionBlockCodeAttribute : Attribute
{
    /// <summary>
    /// The op code
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Creates a new instance 
    /// </summary>
    /// <param name="code">The op code of the block</param>
    public ExecutionBlockCodeAttribute(string code)
    {
        Code = code;
    }
}
