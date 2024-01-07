using Scratch.Core.Attributes;
using Scratch.Core.Blocks.Operator;

namespace Scratch.Core.Enums;

/// <summary>
/// All operation of the <see cref="Mathop"/> block
/// </summary>
public enum MathopOperation
{
    /// <summary>
    /// absolutes a value
    /// </summary>
    [EnumName("abs")]
    Abs,

    /// <summary>
    /// floors a value
    /// </summary>
    [EnumName("floor")]
    Floor,

    /// <summary>
    /// ceiling a value
    /// </summary>
    [EnumName("ceiling")]
    Ceiling,

    /// <summary>
    /// quare root of a value
    /// </summary>
    [EnumName("sqrt")]
    Sqrt,

    /// <summary>
    /// sine of a value
    /// </summary>
    [EnumName("sin")]
    Sin,

    /// <summary>
    /// cosine of a value
    /// </summary>
    [EnumName("cos")]
    Cos,

    /// <summary>
    /// tangent of a value
    /// </summary>
    [EnumName("tan")]
    Tan,

    /// <summary>
    ///arcsine of a value
    /// </summary>
    [EnumName("asin")]
    Asin,

    /// <summary>
    /// arccosine of a value
    /// </summary>
    [EnumName("acos")]
    Acos,

    /// <summary>
    /// arctangent of a value
    /// </summary>
    [EnumName("atan")]
    Atan,

    /// <summary>
    /// Logarithm
    /// </summary>
    [EnumName("ln")]
    Ln,

    /// <summary>
    /// Logarithm with base 10
    /// </summary>
    [EnumName("log")]
    Log,

    /// <summary>
    /// <see cref="Math.E"/> raised to a power
    /// </summary>
    [EnumName("e ^")]
    PowE,

    /// <summary>
    /// 10 raised to a power
    /// </summary>
    [EnumName("10 ^")]
    Pow10
}
