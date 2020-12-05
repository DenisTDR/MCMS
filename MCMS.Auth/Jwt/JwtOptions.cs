using System;
using Microsoft.IdentityModel.Tokens;

namespace MCMS.Auth.Jwt
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SigningCredentials SignInCredentials { get; set; }
        public DateTime Expiration => IssuedAt.Add(ValidFor);
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime NotBefore { get; set; } = DateTime.UtcNow;
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(360);
    }
}
