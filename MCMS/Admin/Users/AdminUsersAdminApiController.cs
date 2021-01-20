using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Auth;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Base.Data;
using MCMS.Base.Extensions;
using MCMS.Controllers.Api;
using MCMS.Data;
using MCMS.Models.Dt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Admin.Users
{
    [Authorize(Roles = "Admin, Moderator")]
    public class AdminUsersAdminApiController : AdminApiController
    {
        protected IRepository<User> Repo => ServiceProvider.GetRepo<User>();
        protected BaseDbContext DbContext => ServiceProvider.GetRequiredService<BaseDbContext>();
        private IEmailSender EmailSender => ServiceProvider.GetRequiredService<IEmailSender>();

        protected virtual DtQueryService<UserViewModel> QueryService =>
            ServiceProvider.GetService<DtQueryService<UserViewModel>>();

        [AdminApiRoute("~/[controller]")]
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

        [AdminApiRoute("~/[controller]/dtquery")]
        [HttpPost]
        [ModelValidation]
        public virtual async Task<ActionResult<DtResult<UserViewModel>>> DtQuery(
            [FromBody] [Required] DtParameters model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await QueryService.Query(Repo, model);
            result.Data = (await DbContext.Users
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
            return Ok(result);
        }


        [HttpPost]
        [Route("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> ChangeRoles([FromRoute] string id,
            [FromBody] Dictionary<string, object> roles)
        {
            var asMod = !UserFromClaims.HasRole("Admin");
            var userManager = ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await Repo.GetOneOrThrow(id);
            var allRoles = await ServiceProvider.GetRequiredService<RoleManager<Role>>().Roles.Select(role => role.Name)
                .ToListAsync();
            var existingRoles = await userManager.GetRolesAsync(user);
            var newRoles = allRoles.Where(roles.ContainsKey)
                .ToList();
            var toDeleteRoles = existingRoles.Except(newRoles).ToList();

            if (UserFromClaims.Id == id)
            {
                toDeleteRoles = toDeleteRoles.Where(r => r != (asMod ? "Moderator" : "Admin")).ToList();
            }

            var toAddRoles = newRoles.Except(existingRoles).ToList();
            if (asMod)
            {
                if (toAddRoles.Contains("Admin"))
                {
                    toAddRoles.RemoveAll(r => r == "Admin");
                }

                if (toDeleteRoles.Contains("Admin"))
                {
                    toDeleteRoles.RemoveAll(r => r == "Admin");
                }
            }

            await userManager.AddToRolesAsync(user, toAddRoles);
            await userManager.RemoveFromRolesAsync(user, toDeleteRoles);

            return Ok(roles);
        }


        [HttpPost]
        [Route("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> ConfirmEmail([FromRoute] string id)
        {
            // var userManager = ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await Repo.GetOneOrThrow(id);
            user.EmailConfirmed = true;
            await Repo.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> ResendActivationMail([FromRoute] string id)
        {
            var user = await Repo.GetOneOrThrow(id);

            await ServiceProvider.GetRequiredService<AuthService>().SendActivationEmail(user, Url, Request.Scheme);

            return Ok();
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