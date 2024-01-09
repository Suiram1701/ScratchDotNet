using ScratchDotNet.Core;

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
        Metadata metadata = await Setup.Project.GetMetadataAsync();

        Assert.Multiple(() =>
        {
            Assert.That(metadata.Vm, Is.Not.Empty, "Unexpected {0} string at Property {1}", nameof(string.Empty), nameof(Metadata.Vm));
            Assert.That(metadata.UserAgent, Is.Not.Empty, "Unexpected {0} string at Property {1}", nameof(string.Empty), nameof(Metadata.UserAgent));
        });
    }
}
