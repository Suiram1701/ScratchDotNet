using Scratch.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scratch.Core.Blocks.Interfaces;

/// <summary>
/// Provides an value provider with an const value
/// </summary>
internal interface IConstProvider : IValueProvider
{
    /// <summary>
    /// The type of the const value
    /// </summary>
    public DataType DataType { get; internal set; }
}
