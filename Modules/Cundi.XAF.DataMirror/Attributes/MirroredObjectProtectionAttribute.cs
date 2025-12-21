#nullable enable
namespace Cundi.XAF.DataMirror.Attributes;

/// <summary>
/// Controls whether a MirroredObject-derived class is protected from modifications and deletions.
/// When IsProtected is true, the object is read-only in both UI and API.
/// Default value is false (modifications are allowed).
/// </summary>
/// <remarks>
/// Usage:
/// <code>
/// // This class can be modified and deleted (default behavior)
/// public class EditableMirroredEntity : MirroredObject { }
/// 
/// // This class is protected - read-only in UI and API
/// [MirroredObjectProtection(true)]
/// public class ProtectedMirroredEntity : MirroredObject { }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class MirroredObjectProtectionAttribute : Attribute
{
    /// <summary>
    /// Gets whether the object is protected from modifications and deletions.
    /// Default is false (modifications are allowed).
    /// </summary>
    public bool IsProtected { get; }

    /// <summary>
    /// Initializes a new instance with the specified protection setting.
    /// </summary>
    /// <param name="isProtected">True to protect the object from modifications; false to allow modifications.</param>
    public MirroredObjectProtectionAttribute(bool isProtected = true)
    {
        IsProtected = isProtected;
    }

    /// <summary>
    /// Checks if a type is protected based on the MirroredObjectProtectionAttribute.
    /// Returns true only if the attribute is present with IsProtected = true.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is protected; false otherwise.</returns>
    public static bool IsTypeProtected(Type type)
    {
        var attribute = (MirroredObjectProtectionAttribute?)Attribute.GetCustomAttribute(
            type, typeof(MirroredObjectProtectionAttribute), inherit: true);

        return attribute?.IsProtected == true;
    }
}
