using Cundi.XAF.SyncReceiver.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System.ComponentModel;

namespace Cundi.XAF.SyncReceiver;

/// <summary>
/// TypeConverter that provides a list of types inheriting from SyncableObject.
/// Used in the UI to allow selection of only syncable types.
/// </summary>
public class SyncableTypeConverter : LocalizedClassInfoTypeConverter
{
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
        var syncableTypes = new List<Type>();

        // Get all persistent types that inherit from SyncableObject
        foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes)
        {
            if (typeInfo.Type != null &&
                typeInfo.Type != typeof(SyncableObject) &&
                typeof(SyncableObject).IsAssignableFrom(typeInfo.Type) &&
                !typeInfo.Type.IsAbstract)
            {
                syncableTypes.Add(typeInfo.Type);
            }
        }

        // Sort by type name for easier selection
        syncableTypes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

        return new StandardValuesCollection(syncableTypes);
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => true;
}
