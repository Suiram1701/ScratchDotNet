using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;

namespace ScratchDotNet.Core;

public class Project : IDisposable
{
    private bool _disposedValue;

    private readonly ILoggerFactory _loggerFactory;
    private readonly string _tmpDirectoryPath;

    public Project(string filePath) : this(filePath, NullLoggerFactory.Instance)
    {
    }

    public Project(string filePath, ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        ILogger logger = loggerFactory.CreateLogger("Scratch.Core.Project.Initialize");

        ArgumentNullException.ThrowIfNull(filePath, nameof(filePath));
        if (!File.Exists(filePath))
        {
            logger.LogError("Specified .sb3 file not found!; file: {file}", filePath);
            throw new FileNotFoundException("The specified file have to exist.", filePath);
        }

        using IDisposable? readScope = logger.BeginScope("Extract archive");

        // Create tmp folder
        string tmpDirectory = Path.GetTempPath();
        string guid;
        do
        {
            guid = Guid.NewGuid().ToString();
        }
        while (Directory.Exists(Path.Combine(tmpDirectory, guid)));     // Create new tmp directory names until a unique one is found

        _tmpDirectoryPath = Path.Combine(tmpDirectory, guid);
        Directory.CreateDirectory(_tmpDirectoryPath);
        logger.LogInformation("Project extracted to temp directory {directory}", _tmpDirectoryPath);

        using Stream sb3Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using ZipArchive sb3Archive = new(sb3Stream, ZipArchiveMode.Read, false);
        sb3Archive.ExtractToDirectory(_tmpDirectoryPath, false);
        logger.LogInformation("Archive successful extracted");

        string projFilePath = Path.Combine(_tmpDirectoryPath, "project.json");
        if (!File.Exists(projFilePath))
        {
            Dispose();

            logger.LogError("Could not found main project file (invalid project).");
            throw new FileNotFoundException("The project file of the loaded scratch project couldn't be found (invalid project).");
        }
    }

    /// <summary>
    /// Get the metadata of the scratch file
    /// </summary>
    /// <returns>The metadata</returns>
    public async Task<Metadata> GetMetadataAsync(CancellationToken ct = default)
    {
        ThrowIfDisposed();

        ILogger logger = _loggerFactory.CreateLogger("Scratch.Core.Project.ReadMetadata");
        logger.LogInformation("Start reading project metadata");

        string projFilePath = Path.Combine(_tmpDirectoryPath, "project.json");
        if (!File.Exists(projFilePath))
        {
            logger.LogError("Could not found main project file.");
            throw new FileNotFoundException("The project file of the loaded scratch project couldn't be found.");
        }

        using StreamReader streamReader = new(projFilePath);
        JsonReader reader = new JsonTextReader(streamReader);
        JObject obj = await JObject.LoadAsync(reader, ct);

        Metadata metadata = obj["meta"]!.ToObject<Metadata>()!;

        logger.LogInformation("Project metadata successfully read");
        return metadata;
    }

    public async Task<ProjectExecutor> CreateProjectExecutorAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Managed resources
            }

            //Unmanaged resources
            Directory.Delete(_tmpDirectoryPath, true);

            _disposedValue = true;
        }
    }

    ~Project()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Throws an exception of the object is disposed
    /// </summary>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposedValue, this);
    }
}