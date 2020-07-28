using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace MCMS.Admin.Users
{
    public class UserViewModel : ViewModel
    {
        public string UserName { get; set; }
        [TableColumn] public string FullName { get; set; }

        public string Email { get; set; }

        [DisplayName("Email")]
        [DetailsField(Hidden = true)]
        [TableColumn]
        public string EmailColumn => Email + (EmailConfirmed ? "" : " (not confirmed)");

        public bool EmailConfirmed { get; set; }
        public DateTime Created { get; set; }

        [DetailsField(Hidden = true)] public List<string> RolesList { get; set; }
        [TableColumn] public string Roles => string.Join(", ", RolesList?.OrderBy(r => r).ToList() ?? new List<string>());

        public override string ToString()
        {
            return FullName;
        }
    }
}