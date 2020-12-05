using System.Collections.Generic;
using MCMS.Auth.Models;

namespace MCMS.Auth.Jwt
{
    public interface IJwtFactory
    {
        string GenerateToken(string username, IEnumerable<string> roles, string id);
        Session GenerateSession(string username, IEnumerable<string> roles, string id);
    }
}