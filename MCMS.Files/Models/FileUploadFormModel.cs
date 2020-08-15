using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using MCMS.Files.Attributes;
using MCMS.Files.Controllers;

namespace MCMS.Files.Models
{
    public class FileUploadFormModel : IFormModel
    {
        [FormlyFile(typeof(FilesAdminApiController), nameof(FilesAdminApiController.Upload), "admin", "admin")]
        [Required]
        public FileFormModel File { get; set; }

        [DataType(DataType.MultilineText)] public string Description { get; set; }

        [FormlyField(DefaultValue = false)] public bool Protected { get; set; }
    }
}