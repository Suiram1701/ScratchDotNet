using ScratchDotNet.Core.Blocks;
using ScratchDotNet.Core.EventArgs;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Interfaces;

namespace ScratchDotNet.Core.Data;

/// <summary>
/// A variable
/// </summary>
public class Variable
{
    /// <summary>
    /// Called when the value of the variable could be have changed
    /// </summary>
    public event EventHandler<ValueChangedEventArgs>? OnValueChanged;

    /// <summary>
    /// The name of the variable
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The id of this variable
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The value of this variable
    /// </summary>
    public IScratchType Value
    {
        get => _value;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            IScratchType oldItem = _value;
            _value = value;

            OnValueChanged?.Invoke(this, new(oldItem, value));
        }
    }
    private IScratchType _value;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The default value of the variable</param>
    /// <exception cref="ArgumentException"></exception>
    public Variable(string name, IScratchType? value) : this(name, BlockHelpers.GenerateBlockId(), value)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The default value of the variable</param>
    /// <param name="id">The id of the variable</param>
    /// <exception cref="ArgumentException"></exception>
    public Variable(string name, string id, IScratchType? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        Name = name;
        Id = id;
        _value = value ?? new StringValue();
    }

    /// <summary>
    /// Creates a reference instance of this variable
    /// </summary>
    /// <returns></returns>
    public VariableRef CreateReference() =>
        new(this);
}
