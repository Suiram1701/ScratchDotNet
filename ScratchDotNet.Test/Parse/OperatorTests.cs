using ScratchDotNet.Test.StartCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Test.Parse;

/// <summary>
/// Tests for operators
/// </summary>
[TestFixture]
internal class OperatorTests
{
    /// <summary>
    /// Tests for the operator blocks 'add', 'subtract', 'multiply' and 'divide'
    /// </summary>
    [Test]
    public void Arithmetic() =>
        TestHelpers.DoParseTest(OperatorCodes.ArithmeticCode);

    /// <summary>
    /// Tests for the operator blocks 'NOT', 'AND' and 'OR'
    /// </summary>
    [Test]
    public void Logical() =>
        TestHelpers.DoParseTest(OperatorCodes.LogicalCode);

    /// <summary>
    /// Tests for the operator blocks 'larger than', 'less than' and 'equals'
    /// </summary>
    [Test]
    public void Comarison() =>
        TestHelpers.DoParseTest(OperatorCodes.ComparisonCode);

    /// <summary>
    /// Tests for all variants of the operator block 'mathop'
    /// </summary>
    [Test]
    public void Mathop() =>
        TestHelpers.DoParseTest(OperatorCodes.MathopCode);

    /// <summary>
    /// Tests for the operator blocks 'connect', 'char at', 'lengh of' and 'contains'
    /// </summary>
    [Test]
    public void String() =>
        TestHelpers.DoParseTest(OperatorCodes.StringCode);

    /// <summary>
    /// Tests for the blocks 'random', 'modulo' and 'round'
    /// </summary>
    [Test]
    public void Others() =>
        TestHelpers.DoParseTest(OperatorCodes.OthersCode);
}
