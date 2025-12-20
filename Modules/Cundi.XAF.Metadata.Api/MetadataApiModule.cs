#nullable enable
using DevExpress.ExpressApp;

namespace Cundi.XAF.Metadata.Api;

/// <summary>
/// XAF API module that provides Web API endpoints for metadata queries.
/// Extends MetadataModule with REST API functionalities.
/// </summary>
public sealed class MetadataApiModule : ModuleBase
{
    public MetadataApiModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(Cundi.XAF.Metadata.MetadataModule));
    }
}
