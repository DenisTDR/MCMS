using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;

namespace MCMS.Files.Models
{
    public class FileFormModel : IFormModel
    {
        [FormlyIgnore] public string Id { get; set; }

        [FormlyIgnore] public string OwnerToken { get; set; }

        public string OriginalName { get; set; }
        
        public bool Protected { get; set; }

        [DataType(DataType.MultilineText)] public string Description { get; set; }
    }
}