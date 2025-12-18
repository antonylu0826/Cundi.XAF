using Cundi.XAF.FullTextSearch.Attributes;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Sample.Module.BusinessObjects;

[DefaultClassOptions]
[GlobalSearchable]
public class FullTextSearch2 : BaseObject
{
    public FullTextSearch2(Session session) : base(session) { }


    private string _Name;
    public string Name
    {
        get { return _Name; }
        set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
    }


    private string _Description;
    public string Description
    {
        get { return _Description; }
        set { SetPropertyValue<string>(nameof(Description), ref _Description, value); }
    }


    private string _Specification;
    [Size(SizeAttribute.Unlimited)]
    public string Specification
    {
        get { return _Specification; }
        set { SetPropertyValue<string>(nameof(Specification), ref _Specification, value); }
    }




}
