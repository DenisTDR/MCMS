using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using MCMS.Auth.Models;
using MCMS.Base.Auth;
using Microsoft.Extensions.Options;

namespace MCMS.Auth.Jwt
{
    internal class JwtFactory : IJwtFactory
    {
        private readonly JwtOptions _jwtOptions;

        public JwtFactory(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(string username, IEnumerable<string> roles, string id)
        {
            var claims = new List<Claim>
            {
                new(Claims.Username, username),
                new(Claims.Id, id),
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SignInCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Session GenerateSession(User user, IEnumerable<string> roles, string id)
        {
            var username = user.UserName;
            var rolesList = roles.ToList();
            return new Session
            {
                TokenType = "Bearer",
                Fullname = user.FullName,
                Token = GenerateToken(username, rolesList, id),
                Username = username,
                Roles = string.Join(",", rolesList),
                Expiration = DateTime.Now.AddSeconds(_jwtOptions.ValidFor.TotalSeconds)
            };
        }
    }
}