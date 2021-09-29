using System.Linq;
using System.Threading.Tasks;
using MCMS.Auth.Jwt;
using MCMS.Auth.Tokens;
using MCMS.Auth.Tokens.Models;
using MCMS.Base.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MCMS.Auth.Session
{
    public class SessionService : ISessionService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtOptions _jwtOptions;
        protected readonly ILogger Logger;
        public IRefreshTokensService RefreshTokensService { get; }

        public SessionService(
            UserManager<User> userManager,
            IJwtFactory jwtFactory,
            IOptions<JwtOptions> jwtOptions,
            IRefreshTokensService refreshTokensService,
            ILoggerFactory loggerFactory
        )
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            RefreshTokensService = refreshTokensService;
            _jwtOptions = jwtOptions.Value;
            Logger = loggerFactory.CreateLogger("Auth");
        }

        public async Task<SessionDto> CreateSession(User user, string ipAddress = null)
        {
            user = await _userManager.FindByIdAsync(user.Id);
            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            Logger.LogInformation("Creating session for user {UserId}", user.Id);

            var refreshToken = await RefreshTokensService.CreateRefreshToken(user, ipAddress);


            var session = new SessionDto
            {
                Profile = new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Roles = roles,
                    Fullname = user.FullName,
                },
                AccessToken = _jwtFactory.GenerateToken(user.Id, user.UserName, roles, _jwtOptions.CalcExpiration()),
                RefreshToken = TokenDto.FromRefreshTokenEntity(refreshToken)
            };

            return session;
        }

        public Task<SessionDto> CreateSession(string userId, string ipAddress = null)
        {
            return CreateSession(new User { Id = userId }, ipAddress);
        }

        public Task RevokeRefreshToken(string token, string ipAddress = null)
        {
            return RefreshTokensService.RevokeToken(token);
        }


        public async Task<SessionDto> RefreshSession(string token, string ipAddress = null)
        {
            var user = await RefreshTokensService.GetUserByRefreshToken(token);

            var refreshToken = await RefreshTokensService.RecreateRefreshToken(user, token, ipAddress);

            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            var session = new SessionDto
            {
                Profile = new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Roles = roles,
                    Fullname = user.FullName,
                },
                AccessToken = _jwtFactory.GenerateToken(user.Id, user.UserName, roles, _jwtOptions.CalcExpiration()),
                RefreshToken = TokenDto.FromRefreshTokenEntity(refreshToken)
            };

            return session;
        }
    }
}