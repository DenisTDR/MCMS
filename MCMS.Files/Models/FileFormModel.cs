using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace MCMS.Files.Models
{
    public class FileFormModel : IFormModel
    {
        [FormlyField(IgnoreField = true)] public string Id { get; set; }

        [FormlyField(IgnoreField = true)] public string OwnerToken { get; set; }

        public string OriginalName { get; set; }
        
        public bool Protected { get; set; }

        [DataType(DataType.MultilineText)] public string Description { get; set; }
    }
}