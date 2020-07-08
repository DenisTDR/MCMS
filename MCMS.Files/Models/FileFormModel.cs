using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;

namespace MCMS.Files.Models
{
    public class FileFormModel : IFormModel
    {
        // [Required]
        [FormlyIgnore] public string Id { get; set; }

        [FormlyIgnore] public string OwnerToken { get; set; }

        // public string OriginalName { get; set; }
        // public string Name { get; set; }
        // public string Extension { get; set; }
        // public string PhysicalPath { get; set; }
        // public string VirtualPath { get; set; }
        //
        // public bool Claimed { get; set; }
        public bool Protected { get; set; }

        [DataType(DataType.MultilineText)] public string Description { get; set; }
    }
}