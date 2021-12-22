using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Auth.Jwt;
using MCMS.Auth.Tokens.Models;
using MCMS.Base.Auth;
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using MCMS.Base.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MCMS.Auth.Tokens
{
    public class RefreshTokensService : IRefreshTokensService
    {
        private readonly IRepository<User> _usersRepo;
        private readonly IRepository<RefreshTokenEntity> _refreshTokensRepo;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger _logger;

        public RefreshTokensService(
            IOptions<JwtOptions> jwtOptions,
            IRepository<User> usersRepo,
            IRepository<RefreshTokenEntity> refreshTokensRepo,
            ILoggerFactory loggerFactory
        )
        {
            _usersRepo = usersRepo;
            _refreshTokensRepo = refreshTokensRepo;
            _jwtOptions = jwtOptions.Value;
            _logger = loggerFactory.CreateLogger("Auth");
        }

        public async Task<RefreshTokenEntity> CreateRefreshToken(User user, string ipAddress)
        {
            var refreshToken = GenerateRefreshToken(ipAddress);
            _logger.LogInformation("Creating refresh token for user {UserId}", user?.Id);
            user = _usersRepo.Attach(user);
            refreshToken.User = user;
            await RemoveOldRefreshTokens(user);
            await _refreshTokensRepo.Add(refreshToken);

            return refreshToken;
        }

        public async Task RevokeToken(string token, string ipAddress = null)
        {
            var refreshToken = await _refreshTokensRepo.Queryable
                .Where(t => t.Token == token)
                .FirstOrDefaultAsync();

            if (refreshToken == null)
                throw new KnownException("invalid_token_not_found", 404);
            
            if (!refreshToken.IsActive)
                throw new KnownException("invalid_token_not_active");

            _logger.LogInformation("Revoking refresh token {Id}", refreshToken.Id);

            _revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            await _usersRepo.SaveChanges();
        }

        public async Task<RefreshTokenEntity> RecreateRefreshToken(User user, string token, string ipAddress = null)
        {
            var oldToken = await _refreshTokensRepo.GetOne(t => t.Token == token);

            if (oldToken == null)
                throw new KnownException("invalid_token_not_found", 404);

            if (oldToken.IsRevoked)
            {
                var allTokens = await GetAllRefreshTokensForUser(user);
                var msg = $"Attempted reuse of revoked ancestor token: {oldToken.Id}";
                RevokeDescendantRefreshTokens(oldToken, allTokens, ipAddress,
                    $"Attempted reuse of revoked ancestor token: {oldToken.Id}");
                _logger.LogWarning("{Msg}", msg);
                await _usersRepo.SaveChanges();
            }

            if (!oldToken.IsActive)
                throw new KnownException("invalid_token_not_active");

            var newToken = RotateRefreshToken(oldToken, ipAddress);
            newToken.User = user;
            await RemoveOldRefreshTokens(user);
            await _refreshTokensRepo.Add(newToken);
            _logger.LogInformation("Refresh token ({OldTokenId}) to a new token ({NewTokenId})", oldToken.Id,
                newToken.Id);

            return newToken;
        }

        public async Task<User> GetUserByRefreshToken(string token)
        {
            var user = await _refreshTokensRepo.Queryable
                .Where(t => t.Token == token)
                .Select(t => t.User)
                .FirstOrDefaultAsync();
            if (user == null)
                throw new KnownException("invalid_token_not_found", 404);
            return user;
        }

        public async Task<List<RefreshTokenEntity>> GetAllRefreshTokensForUser(User user)
        {
            return await _refreshTokensRepo.GetAll(rt => rt.User == user);
        }

        private RefreshTokenEntity RotateRefreshToken(RefreshTokenEntity oldToken, string ipAddress)
        {
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            _revokeRefreshToken(oldToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void RevokeDescendantRefreshTokens(RefreshTokenEntity refreshToken,
            List<RefreshTokenEntity> allRefreshTokens, string ipAddress,
            string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (string.IsNullOrEmpty(refreshToken.ReplacedByToken)) return;

            var childToken = allRefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
            if (childToken == null) return;
            if (childToken.IsActive)
                _revokeRefreshToken(childToken, ipAddress, reason);
            else
                RevokeDescendantRefreshTokens(childToken, allRefreshTokens, ipAddress, reason);
        }

        private RefreshTokenEntity GenerateRefreshToken(string ipAddress)
        {
            return new RefreshTokenEntity
            {
                Token = Utils.GenerateRandomHexString(64),
                Expires = DateTime.Now.Add(_jwtOptions.RefreshTokenValidFor),
                CreatedByIp = ipAddress
            };
        }

        private static void _revokeRefreshToken(RefreshTokenEntity token, string ipAddress, string reason = null,
            string replacedByToken = null)
        {
            token.Revoked = DateTime.Now;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

        private async Task RemoveOldRefreshTokens(User user)
        {
            var expiredEndpoint = DateTime.Now.Subtract(_jwtOptions.RefreshTokenValidFor * 2);
            await _refreshTokensRepo.Queryable.Where(rt => rt.User == user)
                .Where(rt => rt.Expires < expiredEndpoint)
                .DeleteFromQueryAsync();
        }
    }
}