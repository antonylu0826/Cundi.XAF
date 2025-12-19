namespace Cundi.XAF.SyncReceiver.Services;

/// <summary>
/// Configuration for type mappings in the sync receiver.
/// Maps source system type names to local types.
/// </summary>
public class SyncTypeMappings
{
    private readonly Dictionary<string, Type> _mappings = new();

    /// <summary>
    /// Adds a type mapping from a source type name to a local type.
    /// </summary>
    /// <param name="sourceTypeName">The full type name from the source system</param>
    /// <param name="localType">The local type to map to</param>
    public void AddMapping(string sourceTypeName, Type localType)
    {
        _mappings[sourceTypeName] = localType;
    }

    /// <summary>
    /// Adds a type mapping from a source type name to a local type.
    /// </summary>
    public void AddMapping<TLocal>(string sourceTypeName)
    {
        _mappings[sourceTypeName] = typeof(TLocal);
    }

    /// <summary>
    /// Tries to get the mapped type for a source type name.
    /// </summary>
    public bool TryGetMappedType(string sourceTypeName, out Type? mappedType)
    {
        return _mappings.TryGetValue(sourceTypeName, out mappedType);
    }
}
