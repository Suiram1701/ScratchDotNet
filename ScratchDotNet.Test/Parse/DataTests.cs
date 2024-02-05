using ScratchDotNet.Test.StartCodes;

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

    /// <summary>
    /// Test for the blocks 'add to list', 'delete # of list', 'delete all of list', 'insert ... in list' and 'replace item # of list'
    /// </summary>
    [Test]
    public void Manipulate() =>
        TestHelpers.DoParseTest(DataCodes.ManipulateListCode);

    /// <summary>
    /// Test for the blocks that read data from a list like 'item # of list', 'item # of ... in list', 'length of list' and 'list contains ...?'
    /// </summary>
    [Test]
    public void GetListData() =>
        TestHelpers.DoParseTest(DataCodes.GetListDataCode);
}
