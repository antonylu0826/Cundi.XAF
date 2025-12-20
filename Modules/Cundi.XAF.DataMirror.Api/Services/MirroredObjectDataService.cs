#nullable enable
using Cundi.XAF.DataMirror.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.WebApi.Services;

namespace Cundi.XAF.DataMirror.Api.Services;

/// <summary>
/// Custom DataService that prevents Create/Update/Delete operations on MirroredObject-derived types.
/// MirroredObject types are read-only in the API - they can only be modified via the Mirror API endpoint.
/// </summary>
public class MirroredObjectDataService : DataService
{
    public MirroredObjectDataService(
        IObjectSpaceFactory objectSpaceFactory,
        ITypesInfo typesInfo) : base(objectSpaceFactory, typesInfo)
    {
    }

    /// <summary>
    /// Intercepts ObjectSpace creation to track object changes.
    /// We subscribe to Committing event to block modifications on MirroredObject types.
    /// </summary>
    protected override IObjectSpace CreateObjectSpace(Type objectType)
    {
        // Check if the object type is a MirroredObject-derived type
        if (typeof(MirroredObject).IsAssignableFrom(objectType))
        {
            // Subscribe to Committing event to intercept and block modifications
            var os = base.CreateObjectSpace(objectType);
            os.Committing += Os_Committing;
            return os;
        }

        return base.CreateObjectSpace(objectType);
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
