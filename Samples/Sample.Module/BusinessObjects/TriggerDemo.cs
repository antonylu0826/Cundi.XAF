using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Sample.Module.BusinessObjects;

/// <summary>
/// Test entity for Triggers module.
/// Used to demonstrate trigger rules that fire on object creation, modification, or deletion.
/// Trigger actions can include webhooks, email notifications, etc.
/// </summary>
[DefaultClassOptions]
public class TriggerDemo : BaseObject
{
    public TriggerDemo(Session session) : base(session) { }

    private string _Name;
    public string Name
    {
        get { return _Name; }
        set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
    }


    private string _Note;
    public string Note
    {
        get { return _Note; }
        set { SetPropertyValue<string>(nameof(Note), ref _Note, value); }
    }


}
