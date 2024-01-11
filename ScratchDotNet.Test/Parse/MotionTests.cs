using ScratchDotNet.Test.StartCodes;

namespace ScratchDotNet.Test.Parse;

/// <summary>
/// Tests for motion block parsing
/// </summary>
[TestFixture]
internal class MotionTests
{
    /// <summary>
    /// Tests for "move steps" and "goto"
    /// </summary>
    [Test]
    public void Movement() =>
        TestHelpers.DoParseTest(MotionCodes.MovementCode);

    /// <summary>
    /// Tests for "glide to ..." and "glide to xy"
    /// </summary>
    [Test]
    public void Glidement() =>
        TestHelpers.DoParseTest(MotionCodes.GlidementCode);

    /// <summary>
    /// Tests for "turn", "point in direction" and "point towards"
    /// </summary>
    [Test]
    public void Rotation() =>
        TestHelpers.DoParseTest(MotionCodes.RotationCode);

    /// <summary>
    /// Tests for the movements variables "x-Position", "y-Position" and "direction"
    /// </summary>
    [Test]
    public void MovementVariables() =>
        TestHelpers.DoParseTest(MotionCodes.MovementVariablesCode);

    /// <summary>
    /// Tests for the "set direction type" block
    /// </summary>
    [Test]
    public void SetDirectionType() =>
        TestHelpers.DoParseTest(MotionCodes.SetDirectionTypeCode);

    /// <summary>
    /// Tests for other stuff
    /// </summary>
    [Test]
    public void Others() =>
        TestHelpers.DoParseTest(MotionCodes.OthersCode);
}
