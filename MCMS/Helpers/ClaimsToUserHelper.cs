using System;
using System.Security.Claims;
using MCMS.Base.Auth;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Helpers
{
    public class ClaimsToUserHelper
    {
        private readonly ClaimsPrincipal _claims;
        private readonly UserManager<User> _userManager;

        public ClaimsToUserHelper(ClaimsPrincipal claims, UserManager<User> userManager)
        {
            _claims = claims;
            _userManager = userManager;
        }

        public User GetUserById()
        {
            var userId = _claims.FindFirstValue("Id");
            return _userManager.FindByIdAsync(userId).Result;
        }

        public User GetCurrentUser(bool shouldThrow = true)
        {
            var user = GetUserById();

            if (user == null && shouldThrow)
            {
                throw new Exception("User does not exist.");
            }

            return user;
        }
    }
}