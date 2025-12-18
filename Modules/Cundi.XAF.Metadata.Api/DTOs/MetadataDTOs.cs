namespace Cundi.XAF.Metadata.Api.DTOs
{
    public class MetadataTypeDto
    {
        public string TypeName { get; set; }
        public string FullName { get; set; }
        public string AssemblyName { get; set; }
        public List<MetadataPropertyDto> Properties { get; set; } = new List<MetadataPropertyDto>();
    }

    public class MetadataPropertyDto
    {
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string FriendlyPropertyType { get; set; }
        public string Caption { get; set; }
    }
}
