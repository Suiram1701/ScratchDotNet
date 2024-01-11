using ScratchDotNet.Core;
using System.Diagnostics;

namespace ScratchDotNet.Test;

/// <summary>
/// Some tests for the projects
/// </summary>
[TestFixture]
internal class ProjectTests
{
    /// <summary>
    /// Tests reading of the project metadata
    /// </summary>
    [Test]
    public async Task ReadMetadataAsync()
    {
        Stopwatch sw = Stopwatch.StartNew();
        Metadata metadata = await Setup.Project.GetMetadataAsync();
        sw.Stop();

        Assert.Multiple(() =>
        {
            Assert.That(metadata.Vm, Is.Not.Empty, "Unexpected {0} string at Property {1}", nameof(string.Empty), nameof(Metadata.Vm));
            Assert.That(metadata.UserAgent, Is.Not.Empty, "Unexpected {0} string at Property {1}", nameof(string.Empty), nameof(Metadata.UserAgent));
        });
        Assert.Pass("Needed time: {0}ms", sw.ElapsedMilliseconds);
    }
}
