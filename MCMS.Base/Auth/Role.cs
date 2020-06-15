using System;
using MCMS.Base.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Base.Auth
{
    public class Role : IdentityRole, IEntity
    {
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}