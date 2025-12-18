using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Cundi.XAF.FullTextSearch.Attributes;

namespace Sample.Module.BusinessObjects;

[DefaultClassOptions]
[GlobalSearchable]
public class FullTextSearch1 : BaseObject
{
    public FullTextSearch1(Session session) : base(session) { }


    private string _Name;
    public string Name
    {
        get { return _Name; }
        set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
    }


    private string _Subject;
    public string Subject
    {
        get { return _Subject; }
        set { SetPropertyValue<string>(nameof(Subject), ref _Subject, value); }
    }

    private string _Note;
    [Size(SizeAttribute.Unlimited)]
    public string Note
    {
        get { return _Note; }
        set { SetPropertyValue<string>(nameof(Note), ref _Note, value); }
    }




}
