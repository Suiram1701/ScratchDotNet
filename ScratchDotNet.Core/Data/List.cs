using ScratchDotNet.Core.Blocks;
using ScratchDotNet.Core.Types.Bases;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ScratchDotNet.Core.Data;

/// <summary>
/// Represents a list
/// </summary>
public class List
{
    public event Action? OnValueChanged;

    /// <summary>
    /// The name of the list
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The id of the list
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The list
    /// </summary>
    public ObservableCollection<ScratchTypeBase> Values { get; }

    /// <summary>
    /// Creates a new instance with an empty list
    /// </summary>
    /// <param name="name">The name of the list</param>
    public List(string name) : this(name, values: null)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="name">The name of the list</param>
    /// <param name="values">The content of the list. When <see langword="null"/> an empty list will be created</param>
    public List(string name, IEnumerable<ScratchTypeBase>? values) : this(name, BlockHelpers.GenerateBlockId(), values)
    {
    }

    /// <summary>
    /// Creates a new instance with an empty list
    /// </summary>
    /// <param name="name">The name of the list</param>
    /// <param name="id">The internal id of the list</param>
    public List(string name, string id) : this(name, id, null)
    {
    }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="name">The name of the list</param>
    /// <param name="id">The internal id of the list</param>
    /// <param name="values">The content of the list. When <see langword="null"/> an empty list will be created</param>
    public List(string name, string id, IEnumerable<ScratchTypeBase>? values)
    {
        ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        Name = name;
        Id = id;

        Values = new(values ?? new List<ScratchTypeBase>());
        Values.CollectionChanged += Values_CollectionChanged;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ListRef"/> that referes to this instance
    /// </summary>
    /// <returns>The reference</returns>
    public ListRef CreateReference() =>
        new(this);

    private void Values_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        OnValueChanged?.Invoke();
}
