using ScratchDotNet.Test.StartCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
