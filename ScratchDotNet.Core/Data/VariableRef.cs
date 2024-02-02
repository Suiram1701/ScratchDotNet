using Newtonsoft.Json.Linq;

namespace ScratchDotNet.Core.Data;

/// <summary>
/// A reference to a variable
/// </summary>
public class VariableRef
{
    /// <summary>
    /// The name of the reference variable
    /// </summary>
    public string VarName { get; }

    /// <summary>
    /// The id of the reference variable
    /// </summary>
    public string VarId { get; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="variable">The variable to refer</param>
    /// <exception cref="ArgumentNullException"></exception>
    public VariableRef(Variable variable)
    {
        ArgumentNullException.ThrowIfNull(variable, nameof(variable));

        VarName = variable.Name;
        VarId = variable.Id;
    }

    internal VariableRef(string varName, string varId)
    {
        ArgumentException.ThrowIfNullOrEmpty(varName, nameof(varName));
        ArgumentException.ThrowIfNullOrEmpty(varId, nameof(varId));

        VarName = varName;
        VarId = varId;
    }

    internal VariableRef(JToken blockToken, string jsonPath)
    {
        JToken refToken = blockToken.SelectToken(jsonPath)!;
        VarName = refToken[0]!.Value<string>()!;
        VarId = refToken[1]!.Value<string>()!;
    }
}
