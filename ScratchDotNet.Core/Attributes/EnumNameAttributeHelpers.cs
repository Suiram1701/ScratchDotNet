using Scratch.Core.Attributes;
using System.Reflection;

internal static class EnumNameAttributeHelpers
{

    /// <summary>
    /// Returns the <see cref="EnumNameAttribute"/> value with the enum value
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum</typeparam>
    /// <returns>The dictionary</returns>
    public static Dictionary<string, TEnum> GetNames<TEnum>()
        where TEnum : struct, Enum
    {
        Type enumType = typeof(TEnum);
        TEnum[] values = Enum.GetValues<TEnum>();
        return values.ToDictionary(v =>
        {
            FieldInfo field = enumType.GetField(v.ToString())!;
            return field.GetCustomAttribute<EnumNameAttribute>()?.Name
                ?? v.ToString();
        });
    }

    /// <summary>
    /// Parse the enum with their <see cref="EnumNameAttribute"/> value
    /// </summary>
    /// <param name="string">The string to parse</param>
    /// <typeparam name="TEnum">The type of the enum</typeparam>
    /// <returns>The parsed value</returns>
    public static TEnum ParseEnumWithName<TEnum>(string @string)
        where TEnum : struct, Enum
    {
        if (!TryParseEnumWithName(@string, out TEnum? @enum))
            throw new KeyNotFoundException("No enum value was found for the specified string.");
        return (TEnum)@enum!;
    }

    /// <summary>
    /// Tries to parse the enum with their <see cref="EnumNameAttribute"/> value
    /// </summary>
    /// <param name="string">The string to parse</param>
    /// <param name="enum">The parsed value</param>
    /// <typeparam name="TEnum">The type of the enum</typeparam>
    /// <returns>Was the parse successful or not</returns>
    public static bool TryParseEnumWithName<TEnum>(string? @string, out TEnum? @enum)
        where TEnum : struct, Enum
    {
        foreach ((string name, TEnum enumValue) in GetNames<TEnum>())
        {
            if (@string == name)
            {
                @enum = enumValue;
                return true;
            }
        }

        @enum = null;
        return false;
    }
}