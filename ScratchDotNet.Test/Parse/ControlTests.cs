using ScratchDotNet.Test.StartCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Test.Parse;

/// <summary>
/// Tests for control blocks
/// </summary>
[TestFixture]
internal class ControlTests
{
    /// <summary>
    /// Test for the blocks 'wait' and 'wait until'
    /// </summary>
    [Test]
    public void Wait() =>
        TestHelpers.DoParseTest(ControlCodes.WaitCode);

    /// <summary>
    /// Test fthe blocks 'repeat', 'repeat until' and 'repeat forever'
    /// </summary>
    [Test]
    public void Repeat() =>
        TestHelpers.DoParseTest(ControlCodes.ReapeatCode);

    /// <summary>
    /// Test for the blocks 'if' and 'If else'
    /// </summary>
    [Test]
    public void IfElse() =>
        TestHelpers.DoParseTest(ControlCodes.IfElseCode);

    /// <summary>
    /// Test for the block 'stop this script'
    /// </summary>
    [Test]
    public void StopThisScript() =>
        TestHelpers.DoParseTest(ControlCodes.StopThisSciptCode);

    /// <summary>
    /// Test for the block 'stop all scripts'
    /// </summary>
    [Test]
    public void StopAll() =>
        TestHelpers.DoParseTest(ControlCodes.StopAllCode);

    /// <summary>
    /// Test for the block 'stop other script in sprite'
    /// </summary>
    [Test]
    public void StopOtherScripts() =>
        TestHelpers.DoParseTest(ControlCodes.StopOtherScriptsCode);

    /// <summary>
    /// Test for the blocks 'When I start as a clone', 'create clone of myself', 'create clone of' and 'delete this clone'
    /// </summary>
    [Test]
    public void Clone() =>
        TestHelpers.DoParseTest(ControlCodes.CloneCode);
}
