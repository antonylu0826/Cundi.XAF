using Cundi.XAF.DataMirror.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System.ComponentModel;

namespace Cundi.XAF.DataMirror.TypeConverters;

/// <summary>
/// TypeConverter that provides a list of types inheriting from MirroredObject.
/// Used in the UI to allow selection of only mirrorable types.
/// </summary>
public class MirroredTypeConverter : LocalizedClassInfoTypeConverter
{
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
        var mirroredTypes = new List<Type>();

        // Get all persistent types that inherit from MirroredObject
        foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes)
        {
            if (typeInfo.Type != null &&
                typeInfo.Type != typeof(MirroredObject) &&
                typeof(MirroredObject).IsAssignableFrom(typeInfo.Type) &&
                !typeInfo.Type.IsAbstract)
            {
                mirroredTypes.Add(typeInfo.Type);
            }
        }

        // Sort by type name for easier selection
        mirroredTypes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

        return new StandardValuesCollection(mirroredTypes);
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => true;
}
