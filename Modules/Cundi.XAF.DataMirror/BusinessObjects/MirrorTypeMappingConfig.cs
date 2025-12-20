using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.ComponentModel;
using Cundi.XAF.DataMirror.TypeConverters;

namespace Cundi.XAF.DataMirror.BusinessObjects;

/// <summary>
/// Configuration for mapping source system type names to local types.
/// Allows dynamic configuration of type mappings through the XAF UI.
/// </summary>
[ImageName("BO_Transition")]
[DefaultClassOptions]
[NavigationItem("Configuration")]
public class MirrorTypeMappingConfig : BaseObject
{
    public MirrorTypeMappingConfig(Session session) : base(session) { }

    public override void AfterConstruction()
    {
        base.AfterConstruction();
        IsActive = true;
    }

    private string _sourceTypeName = string.Empty;
    /// <summary>
    /// The full type name from the source system (e.g., "Sample.Module.BusinessObjects.TriggerDemo").
    /// </summary>
    [Size(500)]
    [RuleRequiredField]
    public string SourceTypeName
    {
        get => _sourceTypeName;
        set => SetPropertyValue(nameof(SourceTypeName), ref _sourceTypeName, value);
    }

    private string? _localTypeName;
    /// <summary>
    /// The full type name of the local type (stored in database).
    /// </summary>
    [Browsable(false)]
    public string? LocalTypeName
    {
        get => _localTypeName;
        set
        {
            if (SetPropertyValue(nameof(LocalTypeName), ref _localTypeName, value))
            {
                // Sync LocalType when LocalTypeName changes
                if (!string.IsNullOrEmpty(value))
                {
                    _localType = XafTypesInfo.Instance.FindTypeInfo(value)?.Type;
                }
                else
                {
                    _localType = null;
                }
            }
        }
    }

    private Type? _localType;
    /// <summary>
    /// The local type to map to (UI display only).
    /// Uses MirroredTypeConverter to show only MirroredObject subclasses.
    /// </summary>
    [NonPersistent]
    [ImmediatePostData]
    [RuleRequiredField]
    [TypeConverter(typeof(MirroredTypeConverter))]
    [ModelDefault("Caption", "Local Type")]
    public Type? LocalType
    {
        get => _localType;
        set
        {
            if (_localType != value)
            {
                _localType = value;
                LocalTypeName = value?.FullName;
            }
        }
    }

    private bool _isActive;
    /// <summary>
    /// Whether this mapping is active. Inactive mappings will be ignored.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set => SetPropertyValue(nameof(IsActive), ref _isActive, value);
    }

    private string? _description;
    /// <summary>
    /// Optional description for this mapping.
    /// </summary>
    [Size(500)]
    public string? Description
    {
        get => _description;
        set => SetPropertyValue(nameof(Description), ref _description, value);
    }
}

