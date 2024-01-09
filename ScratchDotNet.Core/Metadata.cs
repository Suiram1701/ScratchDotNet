using Newtonsoft.Json;

namespace ScratchDotNet.Core;

/// <summary>
/// Meta information about the project
/// </summary>
public partial class Metadata
{
    /// <summary>
    /// The scratch version (always 3.0.0)
    /// </summary>
    [JsonProperty("semver")]
    public string Semver { get; set; }

    /// <summary>
    /// The version of the virtual maschine that created that file
    /// </summary>
    [JsonProperty("vm")]
    public string Vm { get; set; }

    /// <summary>
    /// The user agent of the last editor user
    /// </summary>
    [JsonProperty("agent")]
    public string UserAgent { get; set; }

    private const string _constSemver = "3.0.0";

    /// <summary>
    /// Creates a new instance
    /// </summary>
    public Metadata()
    {
        Semver = _constSemver;
        Vm = string.Empty;
        UserAgent = string.Empty;
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="vm">The version of the virtual maschine that created that file</param>
    /// <param name="userAgent">The user agent of the last editor user</param>
    public Metadata(string vm, string userAgent)
    {
        Semver = _constSemver;
        Vm = vm;
        UserAgent = userAgent;
    }
}