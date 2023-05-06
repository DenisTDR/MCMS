using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Admin.Users.Models;
using MCMS.Auth;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using MCMS.Base.Extensions;
using MCMS.Base.Repositories;
using MCMS.Base.SwaggerFormly.Formly.Base;
using MCMS.Controllers.Api;
using MCMS.Data;
using MCMS.Models;
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
        protected BaseDbContext DbContext => Service<BaseDbContext>();
        private IEmailSender EmailSender => Service<IEmailSender>();

        protected virtual DtQueryService<UserViewModel> QueryService =>
            ServiceProvider.GetService<DtQueryService<UserViewModel>>();

        [AdminApiRoute("~/[controller]")]
        [HttpGet]
        public virtual async Task<ActionResult<List<UserViewModel>>> Index()
        {
            Repo.ChainQueryable(q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role));
            var users = await Repo.GetAll();
            var usersVm = Mapper.Map<IList<UserViewModel>>(users);
            return Ok(usersVm);
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

            Repo.ChainQueryable(q => q.Include(u => u.UserRoles).ThenInclude(ur => ur.Role));
            var result = await QueryService.Query(Repo, model);

            return Ok(result);
        }


        [HttpPost]
        [Route("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> ChangeRoles([FromRoute] string id,
            [FromBody] UpdateRolesFormModel model)
        {
            var asMod = !UserFromClaims.HasRole("Admin");
            var userManager = Service<UserManager<User>>();
            var user = await Repo.GetOneOrThrow(id);
            var allRoles = await Service<RoleManager<Role>>().Roles.Select(role => role.Name)
                .ToListAsync();
            var existingRoles = await userManager.GetRolesAsync(user);
            var newRoles = allRoles.Where(role => model.Roles.Contains(role))
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

            return Ok(new
            {
                reloadTable = true
            });
        }


        [HttpPost]
        [Route("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> ConfirmEmail([FromRoute] string id)
        {
            // var userManager = Service<UserManager<User>>();
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

            await Service<AuthService>().SendActivationEmail(user, Url, Request.Scheme);

            return Ok();
        }

        [HttpPost]
        [UseTransaction]
        [ModelValidation]
        public virtual async Task<ActionResult<UserViewModel>> Create([Required] [FromBody] CreateUserFormModel model)
        {
            var roles = model.Roles ?? new List<string>();

            var userManager = Service<UserManager<User>>();

            var user = new User { Email = model.Email, UserName = model.Email };

            var result = await userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                throw new KnownException(result.Errors.First().Description);
            }

            roles.Remove("God");
            if (roles is { Count: > 0 })
            {
                try
                {
                    result = await userManager.AddToRolesAsync(user, roles);
                    if (!result.Succeeded)
                    {
                        throw new KnownException(result.Errors.First().Description);
                    }
                }
                catch (InvalidOperationException exc)
                {
                    throw new KnownException(exc.Message);
                }
            }

            if (model.SendActivationEmail)
            {
                await Service<AuthService>().SendActivationEmail(user, Url, Request.Scheme);
            }

            return Ok(await GetCreateResponseModel(user, roles, model.SendActivationEmail));
        }

        [HttpGet]
        public async Task<ActionResult<List<ValueLabelModel>>> Roles()
        {
            var roles = await Service<RoleManager<Role>>().Roles
                .Select(role => role.Name)
                .Where(role => role != "God").ToListAsync();
            return roles.Select(role => new ValueLabelModel() { Value = role, Label = role }).ToList();
        }

        protected virtual async Task<ModelResponse<CreateUserFormModel>> GetCreateResponseModel(User e,
            List<string> roles, bool sendActivationEmail)
        {
            var fm = new CreateUserFormModel
            {
                Email = e.Email, Roles = roles,
                SendActivationEmail = sendActivationEmail
            };
            var vm = MapV(e);
            var response = new FormSubmitResponse<CreateUserFormModel, UserViewModel>(fm, vm, e.Id)
            {
                Snack = await Service<ITranslationsRepository>().GetValueOrSlug("saved"),
                SnackType = "success",
                SnackDuration = 3000
            };
            return response;
        }

        protected virtual List<UserViewModel> MapV(List<User> entities)
        {
            return Mapper.Map<List<UserViewModel>>(entities);
        }

        protected virtual UserViewModel MapV(User entity)
        {
            return Mapper.Map<UserViewModel>(entity);
        }
    }
}