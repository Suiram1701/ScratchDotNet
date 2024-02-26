namespace ScratchDotNet.Core.Helpers;

/// <summary>
/// A converter that converts the scratch angle format to the ussaly used format and backwards
/// </summary>
/// <param name="angle">The angle to convert</param>
internal readonly struct AngleConverter(double angle)
{
    public readonly double Angle = angle;

    /// <summary>
    /// Converts the specified angle from the normal format to the scratch format
    /// </summary>
    /// <returns>The converted angle</returns>
    /// <exception cref="ArgumentException"></exception>
    public double ConvertToScratchFormat()
    {
        double normalAngle = Angle % 360;     // normalize the angel to a value between 0° and 360°

        if (normalAngle >= 0 && normalAngle <= 180)
            return normalAngle;
        else if (Angle > 180 && normalAngle < 359)
            return normalAngle - 360;

        throw new ArgumentException("The specified angle isn't convertable to scratch angle");
    }

    /// <summary>
    /// Converts the specified angle from the scratch format to the normal format
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public double ConvertToNormalFormat()
    {
        if (Angle >= 0 && Angle <= 180)
            return Angle;
        else if (Angle >= -1 && Angle <= -179)
            return Angle + 360;

        throw new ArgumentException("The specified angle isn't convertable to normal angle.");
    }
}
