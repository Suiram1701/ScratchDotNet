using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScratchDotNet.Core.Types.Interfaces;

/// <summary>
/// An interface that provides convertion and comparatation between every scratch type
/// </summary>
public interface IScratchType : IEquatable<IScratchType>, IComparable<IScratchType>
{
    /// <summary>
    /// Converts this type value into a <see cref="StringValue"/>
    /// </summary>
    /// <returns>The result</returns>
    /// <exception cref="InvalidCastException"></exception>
    public StringValue ConvertToStringValue();

    /// <summary>
    /// Converts this type value into a <see cref="DoubleValue"/>
    /// </summary>
    /// <returns>The result</returns>
    /// <exception cref="InvalidCastException"></exception>
    public DoubleValue ConvertToDoubleValue();
}
