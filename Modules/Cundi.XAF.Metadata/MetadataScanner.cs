using Cundi.XAF.Metadata.BusinessObjects;
using DevExpress.ExpressApp;

namespace Cundi.XAF.Metadata
{
    public static class MetadataScanner
    {
        public static void UpdateMetadata(IObjectSpace objectSpace)
        {
            var types = XafTypesInfo.Instance.PersistentTypes.Where(t => t.IsVisible && t.IsPersistent);

            foreach (var typeInfo in types)
            {
                var metadataType = objectSpace.FindObject<MetadataType>(DevExpress.Data.Filtering.CriteriaOperator.Parse("FullName = ?", typeInfo.FullName));
                if (metadataType == null)
                {
                    metadataType = objectSpace.CreateObject<MetadataType>();
                    metadataType.FullName = typeInfo.FullName;
                }

                metadataType.TypeName = typeInfo.Name;
                metadataType.AssemblyName = typeInfo.AssemblyInfo.Assembly.FullName;

                // Pre-load properties for this type to avoid N+1 queries
                var existingProperties = metadataType.Properties.ToList();

                foreach (var memberInfo in typeInfo.OwnMembers)
                {
                    if (memberInfo.IsPublic && (memberInfo.IsProperty || memberInfo.IsList))
                    {
                        var metadataProperty = existingProperties.FirstOrDefault(p => p.PropertyName == memberInfo.Name);

                        if (metadataProperty == null)
                        {
                            metadataProperty = objectSpace.CreateObject<MetadataProperty>();
                            metadataProperty.Type = metadataType;
                            metadataProperty.PropertyName = memberInfo.Name;
                        }
                        metadataProperty.PropertyType = memberInfo.MemberType.FullName;
                        metadataProperty.Caption = memberInfo.DisplayName;
                    }
                }
            }
            objectSpace.CommitChanges();
        }
    }
}
