using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Cundi.XAF.DataMirror.BusinessObjects;

/// <summary>
/// Abstract base class for mirrored objects.
/// Objects inheriting from this class can be created, modified, and deleted
/// through the mirror API, with the ability to specify the Oid externally
/// to maintain primary key synchronization with the source system.
/// </summary>
[NonPersistent]
[CreatableItem(false)]
public abstract class MirroredObject : XPCustomObject
{
    public MirroredObject(Session session) : base(session) { }

    private Guid _oid = Guid.Empty;

    /// <summary>
    /// The unique identifier (primary key) of the object.
    /// This property can be set externally during object creation
    /// to synchronize with the source system's primary key.
    /// </summary>
    [Key(true)]
    [Persistent(nameof(Oid))]
    public Guid Oid
    {
        get => _oid;
        set => SetPropertyValue(nameof(Oid), ref _oid, value);
    }

    private DateTime? _syncedAt;

    /// <summary>
    /// The timestamp when the object was last mirrored from the source system.
    /// </summary>
    [VisibleInListView(false)]
    [VisibleInDetailView(true)]
    public DateTime? SyncedAt
    {
        get => _syncedAt;
        set => SetPropertyValue(nameof(SyncedAt), ref _syncedAt, value);
    }
}
