using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace MCMS.Admin.Users.Models;

public class UpdateUserNameFormModel : IFormModel
{
    [FormlyField(ClassName = "col-12", Disabled = true)]
    public string OldUserName { get; set; }

    [Required]
    [FormlyField(ClassName = "col-12")]
    public string NewUserName { get; set; }
}