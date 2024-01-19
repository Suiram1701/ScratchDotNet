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
}
