namespace Scratch.Core.Blocks.Attributes;

/// <summary>
/// Set the op code of a block
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class ExecutionBlockCodeAttribute : Attribute
{
    /// <summary>
    /// The Id
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Creates a new instance 
    /// </summary>
    /// <param Id="Id">The op code of the block</param>
    public ExecutionBlockCodeAttribute(string id)
    {
        Id = id;
    }
}
