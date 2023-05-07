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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Admin.Users
{
    [Authorize(Roles = "Admin, Moderator")]
    public class AdminUsersAdminApiController : AdminApiController
    {
        protected IRepository<User> Repo => ServiceProvider.GetRepo<User>();

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
        [ModelValidation]
        public virtual async Task<ActionResult<UserViewModel>> UpdateRoles([FromRoute] string id,
            [FromBody] UpdateRolesFormModel model)
        {
            var asMod = !UserFromClaims.HasRole("Admin");
            var user = await Repo.GetOneOrThrow(id);
            var allRoles = await Service<RoleManager<Role>>().Roles.Select(role => role.Name)
                .ToListAsync();
            allRoles.Remove("God");
            var newRoles = allRoles.Where(role => model.Roles.Contains(role))
                .ToList();

            if (UserFromClaims.Id == id)
            {
                var requiredRole = asMod ? "Moderator" : "Admin";
                if (!newRoles.Contains(requiredRole)) newRoles.Add(requiredRole);
            }

            if (asMod && newRoles.Contains("Admin"))
            {
                newRoles.Remove("Admin");
            }

            await Service<UserService>().UpdateUserRoles(user, newRoles);

            return Ok(new FormSubmitResponse<UpdateRolesFormModel>
            {
                Snack = await Service<ITranslationsRepository>().GetValueOrSlug("updated"),
                SnackType = "success",
                SnackDuration = 3000
            });
        }

        [HttpPost]
        [Route("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> UpdateEmail([FromRoute] string id,
            [Required] [FromBody] UpdateEmailFormModel model)
        {
            model.NewEmail = model.NewEmail.Trim().ToLower();
            if (model.OldEmail == model.NewEmail)
            {
                throw new KnownException("The new email is the same as old email.");
            }

            var userManager = Service<UserManager<User>>();
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (user.Email != model.OldEmail)
            {
                throw new KnownException("Old mail is not the same. Please try again.");
            }


            user.Email = user.UserName = model.NewEmail;
            user.EmailConfirmed = false;

            await userManager.UpdateAsync(user);

            return Ok(new FormSubmitResponse<UpdateEmailFormModel>
            {
                Snack = await Service<ITranslationsRepository>().GetValueOrSlug("updated"),
                SnackType = "success",
                SnackDuration = 3000
            });
        }

        [HttpPost]
        [Route("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> ConfirmEmail([FromRoute] string id)
        {
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
            roles.Remove("God");

            var user = await Service<UserService>().CreateUser(model.Email, null, roles);

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