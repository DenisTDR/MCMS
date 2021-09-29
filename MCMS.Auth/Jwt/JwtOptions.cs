using System;
using Microsoft.IdentityModel.Tokens;

namespace MCMS.Auth.Jwt
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SigningCredentials SignInCredentials { get; set; }
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan RefreshTokenValidFor { get; set; } = TimeSpan.FromDays(1);
        public DateTime CalcExpiration() => DateTime.Now.Add(ValidFor);
    }
}
