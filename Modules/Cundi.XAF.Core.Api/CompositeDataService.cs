#nullable enable
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.WebApi.Services;

namespace Cundi.XAF.Core.Api;

/// <summary>
/// A composite DataService that delegates to multiple IDataServicePlugin implementations.
/// This allows multiple modules to extend the DataService behavior without conflicts.
/// </summary>
public class CompositeDataService : DataService
{
    private readonly IEnumerable<IDataServicePlugin> _plugins;

    public CompositeDataService(
        IObjectSpaceFactory objectSpaceFactory,
        ITypesInfo typesInfo,
        IEnumerable<IDataServicePlugin> plugins, IObjectDeltaHandler deltaHandler)
        : base(objectSpaceFactory, typesInfo, deltaHandler)
    {
        // Order plugins by their Order property (lower values execute first)
        _plugins = plugins.OrderBy(p => p.Order).ToList();
    }

    protected override IObjectSpace CreateObjectSpace(Type objectType)
    {
        var os = base.CreateObjectSpace(objectType);
        
        // Notify all plugins about the new ObjectSpace
        foreach (var plugin in _plugins)
        {
            plugin.OnObjectSpaceCreated(os, objectType);
        }
        
        return os;
    }
}
