using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace MCMS.Admin.Users.Models
{
    public class CreateUserFormModel : IFormModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [FormlyField(ClassName = "col-12 col-sm-6")]
        public string Email { get; set; }

        [FormlyField(DefaultValue = false, ClassName = "col-12 col-sm-6 d-flex align-items-center")]
        public bool SendActivationEmail { get; set; }

        [FormlyCheckList(typeof(AdminUsersAdminApiController), nameof(AdminUsersAdminApiController.Roles),
            ClassName = "col-12")]
        public List<string> Roles { get; set; }
    }
}