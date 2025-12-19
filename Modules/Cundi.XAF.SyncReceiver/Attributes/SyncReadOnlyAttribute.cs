namespace Cundi.XAF.SyncReceiver.Attributes;

/// <summary>
/// Marks a class or property as sync read-only.
/// Objects/properties marked with this attribute can only be modified
/// through the sync API and cannot be edited in the UI.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property,
    AllowMultiple = false, Inherited = true)]
public class SyncReadOnlyAttribute : Attribute
{
    /// <summary>
    /// Gets or sets whether the object/property is read-only.
    /// Default is true.
    /// </summary>
    public bool IsReadOnly { get; set; } = true;

    /// <summary>
    /// Creates a new instance of SyncReadOnlyAttribute with IsReadOnly = true.
    /// </summary>
    public SyncReadOnlyAttribute() { }

    /// <summary>
    /// Creates a new instance of SyncReadOnlyAttribute with the specified IsReadOnly value.
    /// </summary>
    /// <param name="isReadOnly">Whether the object/property is read-only.</param>
    public SyncReadOnlyAttribute(bool isReadOnly)
    {
        IsReadOnly = isReadOnly;
    }
}
