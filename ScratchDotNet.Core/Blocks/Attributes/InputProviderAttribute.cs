using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Blocks.Attributes;

/// <summary>
/// Provides an automatic assignemend of input fields to their json property
/// </summary>
/// <remarks>
/// If <paramref name="name"/> is <c>null</c>, the name of the property with out the suffix -Provider and in upper case will be used as input name
/// </remarks>
/// <param name="name">The name of the input in the json doc</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal class InputProviderAttribute(string? name = null) : Attribute
{
    /// <summary>
    /// The name of the input in the json doc
    /// </summary>
    public string? Name { get; } = name;
}
