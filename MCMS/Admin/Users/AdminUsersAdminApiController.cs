using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MCMS.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersAdminApiController : AdminApiController
    {
        protected IRepository<User> Repo => ServiceProvider.GetService<IRepository<User>>();
        protected BaseDbContext DbContext => ServiceProvider.GetService<BaseDbContext>();

        [AdminApiRoute("/[controller]")]
        [HttpGet]
        public virtual async Task<ActionResult<List<UserViewModel>>> Index()
        {
            var users = (await DbContext.Users
                    .SelectMany(
                        user => DbContext.UserRoles.Where(userRoleMapEntry => user.Id == userRoleMapEntry.UserId)
                            .DefaultIfEmpty(),
                        (user, roleMapEntry) => new {User = user, RoleMapEntry = roleMapEntry})
                    .SelectMany(
                        x => DbContext.Roles.Where(role => role.Id == x.RoleMapEntry.RoleId).DefaultIfEmpty(),
                        (x, role) => new {x.User, Role = role.Name})
                    .ToListAsync())
                .GroupBy(e => e.User)
                .Select(g =>
                {
                    var userVm = MapV(g.Key);
                    userVm.RolesList = g.Select(x => x.Role).ToList();
                    return userVm;
                }).ToList();

            return Ok(users);
        }

        [HttpPost]
        [Route("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> ChangeRoles([FromRoute] string id,
            [FromBody] Dictionary<string, object> roles)
        {
            var userManager = ServiceProvider.GetService<UserManager<User>>();
            var user = await Repo.GetOneOrThrow(id);
            var allRoles = await ServiceProvider.GetService<RoleManager<Role>>().Roles.Select(role => role.Name)
                .ToListAsync();
            var existingRoles = await userManager.GetRolesAsync(user);
            var newRoles = allRoles.Where(roles.ContainsKey)
                .ToList();
            var toDeleteRoles = existingRoles.Except(newRoles);

            if (ServiceProvider.GetService<UserManager<User>>().GetUserId(User) == id)
            {
                toDeleteRoles = toDeleteRoles.Where(r => r != "Admin");
            }

            var toAddRoles = newRoles.Except(existingRoles);
            await userManager.AddToRolesAsync(user, toAddRoles);
            await userManager.RemoveFromRolesAsync(user, toDeleteRoles);

            return Ok(roles);
        }

        protected virtual List<UserViewModel> MapV(List<User> entities)
        {
            return Mapper.Map<List<UserViewModel>>(entities);
        }

        protected virtual UserViewModel MapV(User entities)
        {
            return Mapper.Map<UserViewModel>(entities);
        }
    }
}