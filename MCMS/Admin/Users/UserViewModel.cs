using System;
using System.Collections.Generic;
using System.ComponentModel;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;

namespace MCMS.Admin.Users
{
    [DisplayName("User")]
    public class UserViewModel : ViewModel
    {
        public string UserName { get; set; }

        [TableColumn(DbColumn = "FirstName", DbFuncFormat = "MDbFunctions.Concat({0}, ' ', x.LastName)")]
        public string FullName { get; set; }

        public string Email { get; set; }

        [DisplayName("Email")]
        [DetailsField(Hidden = true)]
        [TableColumn(DbColumn = "Email")]
        public string EmailColumn => Email + (EmailConfirmed ? "" : " (not confirmed)");

        public bool EmailConfirmed { get; set; }
        public DateTime Created { get; set; }

        [TableColumn(Orderable = ServerClient.Client, DbColumn = "UserRoles",
            DbFuncFormat = "{0}.Any(ur=> <condition>)<sel>ur.Role.Name")]
        [DisplayName("Roles")]
        public List<string> RolesList { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}