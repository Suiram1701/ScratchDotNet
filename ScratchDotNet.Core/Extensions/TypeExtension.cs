namespace ScratchDotNet.Core.Extensions;

internal static class TypeExtension
{
    /// <summary>
    /// Do a recursive base type search whether this type is the <paramref name="baseType"/> or this type does direct or indirect inherits from <paramref name="baseType"/>
    /// </summary>
    /// <param name="type">This type</param>
    /// <param name="baseType">The type to search for</param>
    /// <returns><see langword="true"/> when the search was successful</returns>
    public static bool RecursiveTypeBaseTypeSearch(this Type type, Type baseType)
    {
        if (type == baseType)
            return true;

        if (type.BaseType is null)
            return false;
        return type.BaseType.RecursiveTypeBaseTypeSearch(baseType);
    }
}
