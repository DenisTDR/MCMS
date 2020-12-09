using System.Collections.Generic;
using MCMS.Auth.Models;
using MCMS.Base.Auth;

namespace MCMS.Auth.Jwt
{
    public interface IJwtFactory
    {
        string GenerateToken(string username, IEnumerable<string> roles, string id);
        Session GenerateSession(User user, IEnumerable<string> roles, string id);
    }
}