using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Cundi.XAF.Metadata.BusinessObjects;

[CreatableItem(false)]
public class MetadataProperty : BaseObject
{
    public MetadataProperty(Session session) : base(session) { }

    private MetadataType _Type;
    [Association("MetadataType-MetadataProperties")]
    public MetadataType Type
    {
        get { return _Type; }
        set { SetPropertyValue(nameof(Type), ref _Type, value); }
    }

    string propertyName;
    [ModelDefault("AllowEdit", "False")]
    public string PropertyName
    {
        get => propertyName;
        set => SetPropertyValue(nameof(PropertyName), ref propertyName, value);
    }

    string propertyType;
    [ModelDefault("AllowEdit", "False")]
    [Size(SizeAttribute.Unlimited)]
    public string PropertyType
    {
        get => propertyType;
        set => SetPropertyValue(nameof(PropertyType), ref propertyType, value);
    }

    [VisibleInDetailView(false)]
    [VisibleInListView(false)]
    [VisibleInLookupListView(false)]
    public string FriendlyPropertyType
    {
        get
        {
            if (string.IsNullOrEmpty(PropertyType)) return "";
            var type = DevExpress.Persistent.Base.ReflectionHelper.FindType(PropertyType);
            if (type != null)
            {
                return GetFriendlyTypeName(type);
            }
            return PropertyType;
        }
    }

    string caption;
    [ModelDefault("AllowEdit", "False")]
    public string Caption
    {
        get => caption;
        set => SetPropertyValue(nameof(Caption), ref caption, value);
    }

    private string GetFriendlyTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return $"{GetFriendlyTypeName(type.GetGenericArguments()[0])}?";
            }

            string name = type.Name;
            if (name.Contains("`"))
            {
                name = name.Substring(0, name.IndexOf("`"));
            }
            var genericArgs = type.GetGenericArguments().Select(GetFriendlyTypeName);
            return $"{name}<{string.Join(", ", genericArgs)}>";
        }
        return type.Name;
    }

}
