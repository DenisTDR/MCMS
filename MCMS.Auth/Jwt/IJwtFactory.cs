using System;
using System.Collections.Generic;
using MCMS.Auth.Tokens.Models;

namespace MCMS.Auth.Jwt
{
    public interface IJwtFactory
    {
    
        TokenDto GenerateToken(string id, string username, IEnumerable<string> roles, DateTime expiration);
    }
}