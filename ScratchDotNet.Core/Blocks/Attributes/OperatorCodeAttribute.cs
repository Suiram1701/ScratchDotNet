namespace Scratch.Core.Blocks.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal class OperatorCodeAttribute : Attribute
{
    /// <summary>
    /// The Id of this operator
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param id="id">The Id of the operator</param>
    public OperatorCodeAttribute(string id)
    {
        Id = id;
    }
}
