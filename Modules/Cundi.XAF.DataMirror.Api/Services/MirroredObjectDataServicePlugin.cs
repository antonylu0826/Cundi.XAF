#nullable enable
using Cundi.XAF.Core.Api;
using Cundi.XAF.DataMirror.BusinessObjects;
using DevExpress.ExpressApp;

namespace Cundi.XAF.DataMirror.Api.Services;

/// <summary>
/// DataService plugin that prevents Create/Update/Delete operations on MirroredObject-derived types.
/// MirroredObject types are read-only in the API - they can only be modified via the Mirror API endpoint.
/// </summary>
public class MirroredObjectDataServicePlugin : IDataServicePlugin
{
    /// <summary>
    /// High priority (runs first) to block invalid operations early
    /// </summary>
    public int Order => 0;

    public void OnObjectSpaceCreated(IObjectSpace objectSpace, Type objectType)
    {
        // Only subscribe to MirroredObject-derived types
        if (typeof(MirroredObject).IsAssignableFrom(objectType))
        {
            objectSpace.Committing += Os_Committing;
        }
    }

    /// <summary>
    /// Intercepts object commits and blocks Create/Update/Delete on MirroredObject types.
    /// </summary>
    private void Os_Committing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (sender is not IObjectSpace os) return;

        // Check all modified objects
        foreach (var obj in os.ModifiedObjects)
        {
            if (obj is MirroredObject)
            {
                throw new InvalidOperationException(
                    $"Cannot modify '{obj.GetType().Name}': MirroredObject types are read-only in the API. " +
                    $"Use the Mirror API endpoint (/api/Mirror) to synchronize data from the source system.");
            }
        }

        // Check objects to delete
        foreach (var obj in os.GetObjectsToDelete(true))
        {
            if (obj is MirroredObject)
            {
                throw new InvalidOperationException(
                    $"Cannot delete '{obj.GetType().Name}': MirroredObject types are read-only in the API. " +
                    $"Use the Mirror API endpoint (/api/Mirror) to synchronize data from the source system.");
            }
        }
    }
}
