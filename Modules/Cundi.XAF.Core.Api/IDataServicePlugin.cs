#nullable enable
using DevExpress.ExpressApp;

namespace Cundi.XAF.Core.Api;

/// <summary>
/// Interface for DataService plugins that can extend the DataService functionality.
/// Modules implement this interface to add custom behavior when ObjectSpace is created.
/// </summary>
public interface IDataServicePlugin
{
    /// <summary>
    /// Called when an ObjectSpace is created for API operations.
    /// Plugins can subscribe to ObjectSpace events (Committing, Committed, etc.) here.
    /// </summary>
    /// <param name="objectSpace">The newly created ObjectSpace</param>
    /// <param name="objectType">The type of object being operated on</param>
    void OnObjectSpaceCreated(IObjectSpace objectSpace, Type objectType);

    /// <summary>
    /// Execution order of this plugin. Lower values execute first.
    /// Default is 0. Use negative values for high priority, positive for lower priority.
    /// </summary>
    int Order => 0;
}
