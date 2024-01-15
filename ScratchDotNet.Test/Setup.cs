using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core;
using ScratchDotNet.Test.Logger;

namespace ScratchDotNet.Test;

[SetUpFixture]
internal class Setup
{
    public static Project Project { get; private set; }

    public static JObject BlockTokens { get; private set; }

    public static ILoggerFactory LoggerFactory { get; private set; }

    private const string _projectPath = "Sample/Scratch-Projekt.sb3";
    private const string _fileName = "Sample/project.json";

    [OneTimeSetUp]
    public void SetupLogging()
    {
        LoggerFactory = new LoggerFactory(new[]
        {
            new NUnitLoggerProvider()
        });
    }

    [OneTimeSetUp]
    public void ProjectSetup()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _projectPath);
        Project = new Project(filePath, LoggerFactory);
    }

    [OneTimeSetUp]
    public async Task SourceSetupAsync()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileName);
        using StreamReader fileReader = new(filePath);
        using JsonReader jsonReader = new JsonTextReader(fileReader);

        JObject rootObj = await JObject.LoadAsync(jsonReader);
        using JsonReader blocksReader = rootObj.SelectToken("targets[1].blocks")!.CreateReader();

        BlockTokens = await JObject.LoadAsync(blocksReader);
    }

    [OneTimeTearDown]
    public void Dispose()
    {
        Project?.Dispose();
        LoggerFactory?.Dispose();
    }
}
