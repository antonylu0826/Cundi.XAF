using Cundi.XAF.DataMirror.BusinessObjects;
using DevExpress.ExpressApp;

namespace Cundi.XAF.DataMirror.Services;

/// <summary>
/// Configuration for type mappings in the data mirror.
/// Maps source system type names to local types.
/// Supports both database configuration and code-based registration.
/// </summary>
public class MirrorTypeMappings
{
    private readonly Dictionary<string, Type> _codeMappings = new();
    private readonly INonSecuredObjectSpaceFactory? _objectSpaceFactory;

    /// <summary>
    /// Creates a new instance for code-based registration only.
    /// </summary>
    public MirrorTypeMappings()
    {
    }

    /// <summary>
    /// Creates a new instance with database support.
    /// </summary>
    public MirrorTypeMappings(INonSecuredObjectSpaceFactory objectSpaceFactory)
    {
        _objectSpaceFactory = objectSpaceFactory;
    }

    /// <summary>
    /// Adds a type mapping from a source type name to a local type (code-based).
    /// This is a fallback when database configuration is not available.
    /// </summary>
    /// <param name="sourceTypeName">The full type name from the source system</param>
    /// <param name="localType">The local type to map to</param>
    public void AddMapping(string sourceTypeName, Type localType)
    {
        _codeMappings[sourceTypeName] = localType;
    }

    /// <summary>
    /// Adds a type mapping from a source type name to a local type (code-based).
    /// </summary>
    public void AddMapping<TLocal>(string sourceTypeName)
    {
        _codeMappings[sourceTypeName] = typeof(TLocal);
    }

    /// <summary>
    /// Tries to get the mapped type for a source type name.
    /// Priority: Database configuration > Code-based registration
    /// </summary>
    public bool TryGetMappedType(string sourceTypeName, out Type? mappedType)
    {
        // First, try database configuration
        if (_objectSpaceFactory != null)
        {
            try
            {
                using var objectSpace = _objectSpaceFactory.CreateNonSecuredObjectSpace(typeof(MirrorTypeMappingConfig));
                var config = objectSpace.FindObject<MirrorTypeMappingConfig>(
                    DevExpress.Data.Filtering.CriteriaOperator.Parse(
                        "SourceTypeName = ? AND IsActive = True", sourceTypeName));

                if (config != null && !string.IsNullOrEmpty(config.LocalTypeName))
                {
                    mappedType = XafTypesInfo.Instance.FindTypeInfo(config.LocalTypeName)?.Type;
                    if (mappedType != null)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // Fall back to code-based mappings if database access fails
            }
        }

        // Fall back to code-based mappings
        return _codeMappings.TryGetValue(sourceTypeName, out mappedType);
    }

    /// <summary>
    /// Gets all configured mappings from both database and code.
    /// </summary>
    public IEnumerable<(string SourceTypeName, Type LocalType)> GetAllMappings()
    {
        var mappings = new Dictionary<string, Type>(_codeMappings);

        // Add database mappings (override code-based if same key)
        if (_objectSpaceFactory != null)
        {
            try
            {
                using var objectSpace = _objectSpaceFactory.CreateNonSecuredObjectSpace(typeof(MirrorTypeMappingConfig));
                var configs = objectSpace.GetObjects<MirrorTypeMappingConfig>(
                    DevExpress.Data.Filtering.CriteriaOperator.Parse("IsActive = True"));

                foreach (var config in configs)
                {
                    if (!string.IsNullOrEmpty(config.LocalTypeName))
                    {
                        var type = XafTypesInfo.Instance.FindTypeInfo(config.LocalTypeName)?.Type;
                        if (type != null)
                        {
                            mappings[config.SourceTypeName] = type;
                        }
                    }
                }
            }
            catch
            {
                // Ignore database errors
            }
        }

        return mappings.Select(kv => (kv.Key, kv.Value));
    }
}
