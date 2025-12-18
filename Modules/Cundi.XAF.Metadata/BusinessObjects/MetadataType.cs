using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Cundi.XAF.Metadata.BusinessObjects
{
    [DefaultClassOptions]
    [CreatableItem(false)]
    public class MetadataType : BaseObject
    {
        public MetadataType(Session session) : base(session) { }

        string typeName;
        [ModelDefault("AllowEdit", "False")]
        public string TypeName
        {
            get => typeName;
            set => SetPropertyValue(nameof(TypeName), ref typeName, value);
        }

        string fullName;
        [Size(SizeAttribute.Unlimited)]
        [ModelDefault("AllowEdit", "False")]
        public string FullName
        {
            get => fullName;
            set => SetPropertyValue(nameof(FullName), ref fullName, value);
        }

        string assemblyName;
        [Size(SizeAttribute.Unlimited)]
        [ModelDefault("AllowEdit", "False")]
        public string AssemblyName
        {
            get => assemblyName;
            set => SetPropertyValue(nameof(AssemblyName), ref assemblyName, value);
        }

        [DevExpress.Xpo.Aggregated, Association("MetadataType-MetadataProperties")]
        public XPCollection<MetadataProperty> Properties
        {
            get { return GetCollection<MetadataProperty>(nameof(Properties)); }
        }
    }
}
