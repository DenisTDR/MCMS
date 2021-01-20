
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Base.Auth
{
    [Table("AspNetUserRoles2")]
    public class UserRole: IdentityUserRole<string>
    {
        
    }
}