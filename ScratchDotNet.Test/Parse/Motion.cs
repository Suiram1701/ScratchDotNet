namespace ScratchDotNet.Test.Parse;

/// <summary>
/// Tests for motion block parsing
/// </summary>
[TestFixture]
internal class Motion
{
    /// <summary>
    /// Tests for "move steps" and "goto"
    /// </summary>
    [Test]
    public void Movement() =>
        TestHelpers.DoParseTest("x;aM?C]:tC.N`*+RBhAp");

    /// <summary>
    /// Tests for "glide to ..." and "glide to xy"
    /// </summary>
    [Test]
    public void Glidement() =>
        TestHelpers.DoParseTest("fo/@)=_e8hB5qW?e#U#w");

    /// <summary>
    /// Tests for "turn", "point in direction" and "point towards"
    /// </summary>
    [Test]
    public void Rotation() =>
        TestHelpers.DoParseTest("Qy)s^Bz3_v,d#YjqWO$^");

    /// <summary>
    /// Tests for the movements variables "x-Position", "y-Position" and "direction"
    /// </summary>
    [Test]
    public void MovementVariables() =>
        TestHelpers.DoParseTest("]y?=X3TZuVt..ZUXu?jV");

    /// <summary>
    /// Tests for the "set direction type" block
    /// </summary>
    [Test]
    public void SetDirectionType() =>
        TestHelpers.DoParseTest("aBjFw6ijhd/M:(-SxMeX");

    /// <summary>
    /// Tests for other stuff
    /// </summary>
    [Test]
    public void Others() =>
        TestHelpers.DoParseTest("k(RVBDcsL%@Y80dv`kdW");
}
