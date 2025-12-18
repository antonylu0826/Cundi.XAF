using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace Cundi.XAF.Triggers.Helpers;

/// <summary>
/// Helper class to globally disable deferred deletion (soft delete) for XPO objects.
/// This ensures that objects are physically deleted from database instead of being marked with GCRecord.
/// </summary>
public static class DeferredDeletionHelper
{
    private static bool _isInitialized = false;

    /// <summary>
    /// Disables deferred deletion for all XPO objects inheriting from XPCustomObject.
    /// This method is idempotent - calling it multiple times has no additional effect.
    /// </summary>
    public static void DisableDeferredDeletion()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        var infoSource = XpoTypesInfoHelper.GetXpoTypeInfoSource();
        infoSource.XPDictionary.ClassInfoChanged += XPDictionary_ClassInfoChanged;

        var classInfo = infoSource.XPDictionary.Classes
            .Cast<XPClassInfo>()
            .FirstOrDefault(ci => ci.ClassType == typeof(XPCustomObject));

        DisableDeferredDeletionForClass(classInfo);
    }

    private static void XPDictionary_ClassInfoChanged(object sender, ClassInfoEventArgs e)
    {
        if (e.ClassInfo.ClassType == typeof(XPCustomObject))
        {
            var dictionary = (XPDictionary)sender;
            dictionary.ClassInfoChanged -= XPDictionary_ClassInfoChanged;
            DisableDeferredDeletionForClass(e.ClassInfo);
        }
    }

    private static void DisableDeferredDeletionForClass(XPClassInfo? classInfo)
    {
        if (classInfo == null) return;

        var attribute = classInfo.GetAttributeInfo(typeof(DeferredDeletionAttribute))
                        as DeferredDeletionAttribute;
        if (attribute != null)
        {
            attribute.Enabled = false;
        }
    }
}
