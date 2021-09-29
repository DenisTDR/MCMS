using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Auth.Tokens.Models;
using MCMS.Base.Auth;

namespace MCMS.Auth.Tokens
{
    public interface IRefreshTokensService
    {
        public Task<RefreshTokenEntity> CreateRefreshToken(User user, string ipAddress);
        public Task RevokeToken(string token, string ipAddress = null);
        public Task<RefreshTokenEntity> RecreateRefreshToken(User user, string token, string ipAddress = null);
        public Task<User> GetUserByRefreshToken(string token);
        public Task<List<RefreshTokenEntity>> GetAllRefreshTokensForUser(User user);
    }
}