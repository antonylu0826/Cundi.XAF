using Cundi.XAF.DataMirror.Attributes;
using Cundi.XAF.DataMirror.BusinessObjects;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace Receiver.Module.BusinessObjects;

/// <summary>
/// Mirrored TriggerDemo object that receives data from the source Sample.WebApi system.
/// This object is a copy of Sample.Module.BusinessObjects.TriggerDemo,
/// synchronized via webhook from the Cundi.XAF.Triggers module.
/// Protected from modifications - read-only in both UI and API.
/// </summary>
[DefaultClassOptions]
[ImageName("Action_Inline_Edit")]
[MirroredObjectProtection(true)]
public class SyncedTriggerDemo : MirroredObject
{
    public SyncedTriggerDemo(Session session) : base(session) { }

    private string _name;
    /// <summary>
    /// The name of the demo object, synchronized from the source system.
    /// </summary>
    [Size(200)]
    public string Name
    {
        get => _name;
        set => SetPropertyValue(nameof(Name), ref _name, value);
    }
}
