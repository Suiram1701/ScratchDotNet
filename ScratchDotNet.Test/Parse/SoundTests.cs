using ScratchDotNet.Test.StartCodes;

namespace ScratchDotNet.Test.Parse;

/// <summary>
/// Tests for all sound blocks
/// </summary>
[TestFixture]
internal class SoundTests
{
    /// <summary>
    /// Tests for the blocks 'play until done', 'play' and 'stop'
    /// </summary>
    [Test]
    public void Play() =>
        TestHelpers.DoParseTest(SoundCodes.PlayCode);

    /// <summary>
    /// Tests for the blocks 'change effect ... by', 'set effect ... to' and 'clear all effects'
    /// </summary>
    [Test]
    public void Effect() =>
        TestHelpers.DoParseTest(SoundCodes.EffectCode);

    /// <summary>
    /// Tests for the blocks 'change volume by', 'set volume to' and 'get volume'
    /// </summary>
    [Test]
    public void Volume() =>
        TestHelpers.DoParseTest(SoundCodes.VolumeCode);
}
