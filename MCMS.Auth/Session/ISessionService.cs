using System.Threading.Tasks;
using MCMS.Auth.Tokens;
using MCMS.Auth.Tokens.Models;
using MCMS.Base.Auth;

namespace MCMS.Auth.Session
{
    public interface ISessionService
    {
        public IRefreshTokensService RefreshTokensService { get; }
        public Task<SessionDto> CreateSession(User user, string ipAddress = null);

        public Task<SessionDto> CreateSession(string userId, string ipAddress = null);
        public Task RevokeRefreshToken(string token, string ipAddress = null);
        public Task<SessionDto> RefreshSession(string token, string ipAddress = null);
    }
}