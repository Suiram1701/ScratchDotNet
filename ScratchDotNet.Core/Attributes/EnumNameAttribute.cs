namespace Scratch.Core.Attributes;

/// <summary>
/// A attribute used to mark an enum value with a name
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
internal sealed class EnumNameAttribute : Attribute
{
    /// <summary>
    /// The name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="name">The name to set</param>
    public EnumNameAttribute(string name)
    {
        Name = name;
    }
}
