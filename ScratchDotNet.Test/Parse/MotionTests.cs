using ScratchDotNet.Test.StartCodes;

namespace ScratchDotNet.Test.Parse;

/// <summary>
/// Tests for motion block parsing
/// </summary>
[TestFixture]
internal class MotionTests
{
    /// <summary>
    /// Test for the 'move steps' block
    /// </summary>
    [Test]
    public void MoveSteps() =>
        TestHelpers.DoParseTest(MotionCodes.MoveStepsCode);

    /// <summary>
    /// Test for the blocks 'goto' and 'goto xy'
    /// </summary>
    [Test]
    public void Goto() =>
        TestHelpers.DoParseTest(MotionCodes.GotoCode);

    /// <summary>
    /// Test the blocks 'set x', 'change x', 'set y' and 'change y'
    /// </summary>
    [Test]
    public void ChangeXYPosition() =>
        TestHelpers.DoParseTest(MotionCodes.ChangeXYPositionCode);

    /// <summary>
    /// Test for the block 'bouce on edge'
    /// </summary>
    [Test]
    public void BounceOnEdge() =>
        TestHelpers.DoParseTest(MotionCodes.BounceOnEdgeCode);

    /// <summary>
    /// Test for the value blocks 'x position', 'y position' and 'direction'
    /// </summary>
    [Test]
    public void MotionVariables() =>
        TestHelpers.DoParseTest(MotionCodes.MotionVariablesCode);

    /// <summary>
    /// Test for the blocks 'glide in ... s to' and 'glide in ... s to XY'
    /// </summary>
    [Test]
    public void Glide() =>
        TestHelpers.DoParseTest(MotionCodes.GlideCode);

    /// <summary>
    /// Test for the blocks 'turn right', 'turn left', 'point in direction' and 'point towards'
    /// </summary>
    [Test]
    public void Rotation() =>
        TestHelpers.DoParseTest(MotionCodes.RotationCode);

    /// <summary>
    /// Test for the blocks 'set rotation style'
    /// </summary>
    [Test]
    public void SetRotationStyle() =>
        TestHelpers.DoParseTest(MotionCodes.SetRotationStyleCode);
}
