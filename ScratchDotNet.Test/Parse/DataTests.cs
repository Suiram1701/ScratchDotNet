using ScratchDotNet.Test.StartCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Test.Parse;

/// <summary>
/// Tests for variables and lists
/// </summary>
[TestFixture]
public class DataTests
{
    /// <summary>
    /// Tests for the blocks 'set variable to' and 'change variable by'
    /// </summary>
    [Test]
    public void SetVariable() =>
        TestHelpers.DoParseTest(DataCodes.SetVariableCode);

    /// <summary>
    /// Tests for the blocks 'show variable' and 'hide variable'
    /// </summary>
    [Test]
    public void VariableVisibility() =>
        TestHelpers.DoParseTest(DataCodes.VariableVisibilityCode);

    /// <summary>
    /// Test for the variable data type
    /// </summary>
    [Test]
    public void GetVariable() =>
        TestHelpers.DoParseTest(DataCodes.GetVariableCode);
}
