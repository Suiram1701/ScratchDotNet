namespace ScratchDotNet.Core.Blocks.Attributes;

/// <summary>
/// Provides an automatic assignment of a substacks blocks to the property
/// </summary>
/// <remarks>
/// If <paramref name="name"/> is <c>null</c>, the name of the property name in upper case will be used as input name
/// </remarks>
/// <param name="name">The name of the substack in the json doc</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
internal class SubstackAttribute(string? name = null) : Attribute
{
    /// <summary>
    /// The name of the substack in the json doc
    /// </summary>
    public string? Name { get; } = name;
}
