using System.Drawing;

namespace ScratchDotNet.Core.Types.Bases;

/// <summary>
/// The base for a with scratch compatible type (used for dynamic conversion between the types)
/// </summary>
public abstract class ScratchTypeBase : IEquatable<ScratchTypeBase>, IComparable<ScratchTypeBase>
{
    /// <summary>
    /// Get the number value of this type
    /// </summary>
    /// <returns>The number</returns>
    /// <exception cref="NotSupportedException"></exception>
    public abstract double GetNumberValue();

    /// <summary>
    /// Get the string value of this type
    /// </summary>
    /// <returns>The string</returns>
    /// <exception cref="NotSupportedException"></exception>
    public abstract string GetStringValue();

    /// <summary>
    /// Get the bool value of this type
    /// </summary>
    /// <returns>The bool</returns>
    /// <exception cref="NotSupportedException"></exception>
    public abstract bool GetBoolValue();

    /// <summary>
    /// Get the color of this type
    /// </summary>
    /// <returns>The color</returns>
    /// <exception cref="NotSupportedException"></exception>
    public abstract Color GetColorValue();

    public virtual bool Equals(ScratchTypeBase? other) =>
        CompareTo(other) == 0;

    public override bool Equals(object? obj) =>
        Equals(obj as ScratchTypeBase);

    public virtual int CompareTo(ScratchTypeBase? other)
    {
        double value = GetNumberValue();
        double otherValue = other?.GetNumberValue()
            ?? 0d;

        return value.CompareTo(otherValue);
    }

    public override int GetHashCode() =>
        base.GetHashCode();

    public override string ToString() =>
        GetStringValue();

    /// <summary>
    /// Throws an exception that indicates that the conversion to a specified type isn't supported.
    /// </summary>
    /// <param name="type">The target convert type</param>
    protected void ThrowNotSupportedException(Type type)
    {
        string typeName = GetType().Name;
        string targetTypeName = type.Name;
        string message = string.Format("Type {0} don't support conversion to type {1}", typeName, targetTypeName);
        throw new NotSupportedException(message);
    }
}
