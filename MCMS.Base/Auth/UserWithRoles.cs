using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCMS.Base.Auth
{
    [NotMapped]
    public class UserWithRoles : User
    {
        public List<string> Roles { get; set; }

        public bool HasRole(string role)
        {
            return Roles?.Contains(role) == true;
        }
    }
}