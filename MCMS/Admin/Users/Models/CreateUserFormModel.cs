using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace MCMS.Admin.Users.Models
{
    public class CreateUserFormModel : IFormModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Description = "Comma separated list of existing roles")]
        public string Roles { get; set; }

        [FormlyField(DefaultValue = false)] public bool SendActivationEmail { get; set; }
    }
}