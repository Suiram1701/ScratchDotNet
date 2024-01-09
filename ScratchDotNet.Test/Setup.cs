using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScratchDotNet.Core;
using ScratchDotNet.Test.Logger;

namespace ScratchDotNet.Test;

[SetUpFixture]
internal class Setup
{
    public static JObject BlocksToken { get; private set; }

    public static ILoggerFactory LoggerFactory { get; private set; }

    private const string _fileName = "Sample/project.json";

    [OneTimeSetUp]
    public async Task SourceSetupAsync()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileName);
        using StreamReader fileReader = new(filePath);
        using JsonReader jsonReader = new JsonTextReader(fileReader);

        JObject rootObj = await JObject.LoadAsync(jsonReader);
        using JsonReader blocksReader = rootObj.SelectToken("targets[1].blocks")!.CreateReader();

        BlocksToken = await JObject.LoadAsync(blocksReader);
    }

    [OneTimeSetUp]
    public void SetupLogging()
    {
        LoggerFactory = new LoggerFactory(new[]
        {
            new NUnitLoggerProvider()
        });
    }
}
