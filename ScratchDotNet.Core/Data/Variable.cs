using ScratchDotNet.Core.Blocks;
using ScratchDotNet.Core.Types;
using ScratchDotNet.Core.Types.Bases;

namespace ScratchDotNet.Core.Data;

/// <summary>
/// A variable
/// </summary>
public class Variable
{
    /// <summary>
    /// Called when the value of the variable could be have changed
    /// </summary>
    public event Action? OnValueChanged;

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
    public ScratchTypeBase Value
    {
        get => _value;
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            _value = value;

            OnValueChanged?.Invoke();
        }
    }
    private ScratchTypeBase _value;

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The default value of the variable</param>
    /// <exception cref="ArgumentException"></exception>
    public Variable(string name, ScratchTypeBase? value) : this(name, BlockHelpers.GenerateBlockId(), value)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The default value of the variable</param>
    /// <param name="id">The id of the variable</param>
    /// <exception cref="ArgumentException"></exception>
    public Variable(string name, string id, ScratchTypeBase? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        Name = name;
        Id = id;
        _value = value
            ?? new StringType();
    }

    /// <summary>
    /// Creates a reference instance of this variable
    /// </summary>
    /// <returns></returns>
    public VariableRef CreateReference() =>
        new(this);
}
