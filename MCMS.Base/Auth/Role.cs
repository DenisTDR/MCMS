using System;
using MCMS.Base.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Base.Auth;

[IgnoreDefaultTypeConfiguration]
public class Role : IdentityRole, IEntity
{
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public string Description { get; set; }
    public int Rank { get; set; } // 1 - strongest, 0 defaults to 10
}