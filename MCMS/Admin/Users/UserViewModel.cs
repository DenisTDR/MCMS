using System;
using System.Collections.Generic;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace MCMS.Admin.Users
{
    public class UserViewModel : ViewModel
    {
        public string UserName { get; set; }
        [TableColumn] public string FullName { get; set; }

        [TableColumn] public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime Created { get; set; }

        [DetailsField(Hidden = true)] public List<string> RolesList { get; set; }
        [TableColumn] public string Roles => string.Join(", ", RolesList ?? new List<string>());
    }
}