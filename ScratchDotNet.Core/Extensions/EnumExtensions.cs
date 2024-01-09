namespace ScratchDotNet.Core.Extensions;

internal static class EnumExtensions
{
    /// <summary>
    /// Checks whether an enum has more than one flag
    /// </summary>
    /// <param name="enum">The enum to check</param>
    /// <returns>The enum has more than one flag</returns>
    public static bool HasAnyFlag(this Enum @enum)
    {
        ArgumentNullException.ThrowIfNull(@enum, nameof(@enum));

        Array values = Enum.GetValues(@enum.GetType());
        return values.Cast<Enum>().Where(@enum.HasFlag).Count() > 1;
    }
}
