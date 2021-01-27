using System;
using System.ComponentModel.DataAnnotations.Schema;
using MCMS.Base.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Base.Auth
{
    [IgnoreDefaultTypeConfiguration]
    public class UserRole : IdentityUserRole<string>, IEntity
    {
        [NotMapped] public string Id { get; set; }
        [NotMapped] public DateTime Created { get; set; }
        [NotMapped] public DateTime Updated { get; set; }
        
        public Role Role { get; set; }
    }
}