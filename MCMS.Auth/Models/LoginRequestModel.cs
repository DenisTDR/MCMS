using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;

namespace MCMS.Auth.Models
{
    public class LoginRequestModel : IFormModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(64)]
        public virtual string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public virtual string Password { get; set; }
    }
}