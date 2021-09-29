using System;

namespace MCMS.Auth.Tokens.Models
{
    public class TokenDto
    {
        public string Token { get; set; }
        public string TokenType { get; set; }
        public DateTime Expiration { get; set; }

        public static TokenDto FromRefreshTokenEntity(RefreshTokenEntity refreshToken)
        {
            return new TokenDto
            {
                Token = refreshToken.Token,
                TokenType = "Refresh",
                Expiration = refreshToken.Expires
            };
        }
    }
}