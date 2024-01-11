namespace ScratchDotNet.Core.Blocks.Attributes;

/// <summary>
/// Sets the op code of a operator block
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal class OperatorCodeAttribute : Attribute
{
    /// <summary>
    /// The code of this operator
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="code">The id of the operator</param>
    public OperatorCodeAttribute(string code)
    {
        Code = code;
    }
}
