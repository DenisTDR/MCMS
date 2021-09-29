using System.Collections.Generic;

namespace MCMS.Auth.Tokens.Models
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public List<string> Roles { get; set; }
    }
}