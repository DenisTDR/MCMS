using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Base.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Admin.Users
{
    [Service]
    public class UserRolesService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserRolesService(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> UserHasHigherRank(List<string> rolesUserA, List<string> rolesUserB)
        {
            var rankA = await GetRank(rolesUserA);
            var rankB = await GetRank(rolesUserB);
            return rankA <= rankB; // lower rank is "better"
        }

        public async Task<bool> UserHasHigherRank(List<string> rolesUserA, string idUserB)
        {
            var rankA = await GetRank(rolesUserA);
            var rankB = await GetRank(idUserB);
            return rankA <= rankB;
        }

        public async Task<int> GetRank(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KnownException("User not found", 404);
            }

            return await GetRank(user);
        }

        public async Task<int> GetRank(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return await GetRank(roles);
        }

        public async Task<int> GetRank(IList<string> roleNames)
        {
            var allRoles = await GetRolesCached();
            var rank = allRoles.Where(role => roleNames.Contains(role.Name))
                .Min(role => role.Rank);
            return rank;
        }

        private static List<Role> _rolesCached;


        public async Task<List<Role>> GetRolesCached()
        {
            return _rolesCached ??= await _roleManager.Roles.AsNoTracking().ToListAsync();
        }

        public void Ensure(bool value)
        {
            if (!value)
            {
                throw new KnownException(
                    "You can't do this. The user you are trying to edit has a higher rank than you.");
            }
        }
    }
}