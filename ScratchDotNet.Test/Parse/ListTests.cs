using ScratchDotNet.Test.StartCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Test.Parse;

/// <summary>
/// Parsing zu tests for all list blocks
/// </summary>
[TestFixture]
internal class ListTests
{
    /// <summary>
    /// Test for the blocks 'add to list', 'delete # of list', 'delete all of list', 'insert ... in list' and 'replace item # of list'
    /// </summary>
    [Test]
    public void Manipulate() =>
        TestHelpers.DoParseTest(ListCodes.ManipulateCode);
}
