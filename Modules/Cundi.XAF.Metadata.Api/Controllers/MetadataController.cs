using Cundi.XAF.Metadata.Api.DTOs;
using Cundi.XAF.Metadata.BusinessObjects;
using DevExpress.ExpressApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cundi.XAF.Metadata.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MetadataController : ControllerBase
    {
        private readonly IObjectSpaceFactory _objectSpaceFactory;

        public MetadataController(IObjectSpaceFactory objectSpaceFactory)
        {
            _objectSpaceFactory = objectSpaceFactory;
        }

        [HttpGet("Types")]
        public IActionResult GetTypes()
        {
            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<MetadataType>())
            {
                var types = objectSpace.GetObjectsQuery<MetadataType>()
                    .Select(t => new MetadataTypeDto
                    {
                        TypeName = t.TypeName,
                        FullName = t.FullName,
                        AssemblyName = t.AssemblyName
                    })
                    .ToList();
                return Ok(types);
            }
        }

        [HttpGet("Type/{fullName}")]
        public IActionResult GetTypeDetail(string fullName)
        {
            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<MetadataType>())
            {
                var type = objectSpace.GetObjectsQuery<MetadataType>()
                    .FirstOrDefault(t => t.FullName == fullName);

                if (type == null)
                {
                    return NotFound($"Metadata type '{fullName}' not found.");
                }

                var dto = new MetadataTypeDto
                {
                    TypeName = type.TypeName,
                    FullName = type.FullName,
                    AssemblyName = type.AssemblyName,
                    Properties = type.Properties.Select(p => new MetadataPropertyDto
                    {
                        PropertyName = p.PropertyName,
                        PropertyType = p.PropertyType,
                        FriendlyPropertyType = p.FriendlyPropertyType,
                        Caption = p.Caption
                    }).ToList()
                };

                return Ok(dto);
            }
        }
    }
}
