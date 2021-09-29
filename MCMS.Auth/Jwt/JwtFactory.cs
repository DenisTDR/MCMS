using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using MCMS.Auth.Tokens.Models;
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

        public TokenDto GenerateToken(string id, string username, IEnumerable<string> roles, DateTime expiration)
        {
            var claims = new List<Claim>
            {
                new(Claims.Username, username),
                new(Claims.Id, id),
            };
            claims.AddRange(roles.Select(role => new Claim(Claims.Role, role)));

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                DateTime.Now,
                expiration,
                _jwtOptions.SignInCredentials);
            return new TokenDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenType = "Bearer",
                Expiration = expiration,
            };
        }
    }
}