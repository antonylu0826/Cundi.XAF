using Cundi.XAF.SyncReceiver.Attributes;
using Cundi.XAF.SyncReceiver.BusinessObjects;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Receiver.Module.BusinessObjects;

/// <summary>
/// Synced TriggerDemo object that receives data from the source Sample.WebApi system.
/// This object is a copy of Sample.Module.BusinessObjects.TriggerDemo,
/// synchronized via webhook from the Cundi.XAF.Triggers module.
/// </summary>
[DefaultClassOptions]
[SyncReadOnly] // Read-only in UI, can only be modified via sync API
[ImageName("Action_Inline_Edit")]
public class SyncedTriggerDemo : SyncableObject
{
    public SyncedTriggerDemo(Session session) : base(session) { }

    private string? _name;
    /// <summary>
    /// The name of the demo object, synchronized from the source system.
    /// </summary>
    [Size(200)]
    public string? Name
    {
        get => _name;
        set => SetPropertyValue(nameof(Name), ref _name, value);
    }
}
