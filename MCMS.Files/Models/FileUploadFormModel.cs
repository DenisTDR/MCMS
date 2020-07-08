using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Files.Attributes;
using MCMS.Files.Controllers;

namespace MCMS.Files.Models
{
    public class FileUploadFormModel : IFormModel
    {
        [FormlyFile(typeof(FilesAdminApiController), nameof(FilesAdminApiController.Upload))]
        [Required]
        public FileFormModel File { get; set; }

        [DataType(DataType.MultilineText)] public string Description { get; set; }

        [FormlyFieldDefaultValue(false)] public bool Protected { get; set; }
    }
}