namespace Scratch.Core.Extensions;

internal static class TypeExtension
{
    /// <summary>
    /// Do a recursive base type search whether this type does direct or indirect inherits from <paramref name="baseType"/>
    /// </summary>
    /// <param name="type">This type</param>
    /// <param name="baseType">The type to search for</param>
    /// <returns><see langword="true"/> when the search was successful</returns>
    public static bool RecursiveTypeBaseTypeSearch(this Type type, Type baseType)
    {
        if (type.BaseType is null)
            return false;

        if (type.BaseType == baseType)
            return true;

        return type.BaseType.RecursiveTypeBaseTypeSearch(baseType);
    }
}
