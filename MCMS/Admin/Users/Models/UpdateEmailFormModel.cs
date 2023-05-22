using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace MCMS.Admin.Users.Models
{
    public class UpdateEmailFormModel : IFormModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [FormlyField(ClassName = "col-12", Disabled = true)]
        public string OldEmail { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [FormlyField(ClassName = "col-12")]
        public string NewEmail { get; set; }
    }
}