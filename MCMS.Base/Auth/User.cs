using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCMS.Base.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Base.Auth
{
    public class User : IdentityUser, IEntity
    {
        [Display(Name = "First Name")] public string FirstName { get; set; }
        [Display(Name = "Last Name")] public string LastName { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                {
                    return Email;
                }

                return $"{FirstName} {LastName}".Trim();
            }
        }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        [DataType(DataType.EmailAddress)] public override string Email { get; set; }

        [Display(Name = "Email Confirmed")] public override bool EmailConfirmed { get; set; }
        
        // public List<IdentityUserRole> Roles2 { get; set; }
        public List<UserRole> UserRoles { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}