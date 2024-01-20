using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;
using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace MCMS.Admin.Users.Models
{
    public class UpdateUserProfileFormModel : IFormModel
    {
        [FormlyField(ClassName = "col-12")]
        public string FirstName { get; set; }

        [FormlyField(ClassName = "col-12")]
        public string LastName { get; set; }

        [FormlyField(ClassName = "col-12")]
        public string PhoneNumber { get; set; }
    }
}