using System;
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
    /// <summary>
    /// The UserService class provides methods for creating and updating user information, including roles.
    /// </summary>
    [Service]
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Creates a new user with the specified email, password (optional), and roles (optional).
        /// </summary>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="password">The password of the new user (optional). If not provided, the user will be created without a password.</param>
        /// <param name="roles">A list of role names to assign to the new user (optional).</param>
        /// <returns>The newly created user.</returns>
        public async Task<User> CreateUser(string email, string password = null, List<string> roles = null)
        {
            var user = new User
            {
                UserName = email,
                Email = email
            };

            var result = string.IsNullOrEmpty(password)
                ? await _userManager.CreateAsync(user)
                : await _userManager.CreateAsync(user, password);

            EnsureSucceeded(result);

            if (roles != null && roles.Count != 0)
            {
                result = await _userManager.AddToRolesAsync(user, roles);
                EnsureSucceeded(result);
            }


            return user;
        }

        /// <summary>
        /// Updates the roles assigned to the specified user.
        /// </summary>
        /// <param name="user">The user whose roles will be updated.</param>
        /// <param name="roleNames">A list of role names to assign to the user.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public async Task<bool> UpdateUserRoles(User user, IList<string> roleNames)
        {
            var roles = await _roleManager.Roles.ToListAsync();

            // Remove existing roles
            var result = await _userManager.RemoveFromRolesAsync(user, roles.Select(r => r.Name));

            EnsureSucceeded(result);

            // Add new roles
            result = await _userManager.AddToRolesAsync(user, roleNames);

            EnsureSucceeded(result);

            return true;
        }

        /// <summary>
        /// Updates the roles assigned to the user with the specified email address.
        /// </summary>
        /// <param name="email">The email address of the user whose roles will be updated.</param>
        /// <param name="roleNames">A list of role names to assign to the user.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public async Task<bool> UpdateUserRolesByEmail(string email, IList<string> roleNames)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(email));
            }

            return await UpdateUserRoles(user, roleNames);
        }

        /// <summary>
        /// Updates the roles assigned to the user with the specified id.
        /// </summary>
        /// <param name="id">The id address of the user whose roles will be updated.</param>
        /// <param name="roleNames">A list of role names to assign to the user.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public async Task<bool> UpdateUserRoles(string id, IList<string> roleNames)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(id));
            }

            return await UpdateUserRoles(user, roleNames);
        }

        private void EnsureSucceeded(IdentityResult result)
        {
            if (result.Succeeded) return;
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new KnownException($"Error creating user: {errors}");
        }
    }
}