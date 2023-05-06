using System.Collections.Generic;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace MCMS.Admin.Users.Models
{
    public class UpdateRolesFormModel : IFormModel
    {
        [FormlyCheckList(typeof(AdminUsersAdminApiController), nameof(AdminUsersAdminApiController.Roles),
            ClassName = "col-12")]
        public List<string> Roles { get; set; }
    }
}