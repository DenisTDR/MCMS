using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace MCMS.Admin.Users.Models
{
    public class UpdateRolesFormModel : IFormModel
    {
        [FormlyCheckList(typeof(AdminUsersAdminApiController), nameof(AdminUsersAdminApiController.Roles),
            ClassName = "col-12")]
        [Required]
        public List<string> Roles { get; set; }
    }
}