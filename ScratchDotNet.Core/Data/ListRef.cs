using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Data;

/// <summary>
/// Represents a reference to a list
/// </summary>
public class ListRef
{
    /// <summary>
    /// The name of the list to refer to
    /// </summary>
    public string ListName { get; }

    /// <summary>
    /// The id of the variable to refer to
    /// </summary>
    public string ListId { get; }

    /// <summary>
    /// Creates a new instance that refers to the specified list
    /// </summary>
    /// <param name="list">The list to refer to</param>
    public ListRef(List list)
    {
        ArgumentNullException.ThrowIfNull(list, nameof(list));

        ListName = list.Name;
        ListId = list.Id;
    }

    internal ListRef(string listName, string listId)
    {
        ListName = listName;
        ListId = listId;
    }

    internal ListRef(JToken blockToken, string jsonPath)
    {
        JToken refToken = blockToken.SelectToken(jsonPath)!;
        ListName = refToken[0]!.Value<string>()!;
        ListId = refToken[1]!.Value<string>()!;
    }
}
