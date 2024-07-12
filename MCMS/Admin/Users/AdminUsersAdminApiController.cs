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

namespace MCMS.Admin.Users
{
    [Authorize(Roles = "Admin, Moderator")]
    public class AdminUsersAdminApiController : AdminApiController
    {
        protected IRepository<User> Repo => Repo<User>();
        protected UserRolesService UserRolesService => Service<UserRolesService>();

        protected virtual DtQueryService<UserViewModel> QueryService => Service<DtQueryService<UserViewModel>>();

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


        [HttpPost("{id}")]
        [ModelValidation]
        public virtual async Task<ActionResult<UserViewModel>> UpdateRoles([FromRoute] string id,
            [FromBody] UpdateRolesFormModel model)
        {
            // var asMod = !UserFromClaims.HasRole("Admin");
            var user = await Repo.GetOneOrThrow(id);

            UserRolesService.Ensure(await UserRolesService.UserHasHigherRank(UserFromClaims.Roles, id));

            var allRoles = await UserRolesService.GetRolesCached();

            // allRoles.Remove("God");
            var newRoles = allRoles.Where(role => model.Roles.Contains(role.Name))
                .Select(role => role.Name).ToList();

            var canSetThoseRoles = await UserRolesService.UserHasHigherRank(UserFromClaims.Roles, newRoles);
            if (!canSetThoseRoles)
            {
                throw new KnownException("Can't set those roles. Your rank is to low to do that");
            }

            if (UserFromClaims.Id == id)
            {
                if (await UserRolesService.GetRank(UserFromClaims.Roles) != await UserRolesService.GetRank(newRoles))
                {
                    throw new KnownException("You can't downgrade yourself.");
                }
            }

            await Service<UserService>().UpdateUserRoles(user, newRoles.ToList());

            return Ok(new FormSubmitResponse<UpdateRolesFormModel>
            {
                Snack = await Service<ITranslationsRepository>().GetValueOrSlug("updated"),
                SnackType = "success",
                SnackDuration = 3000
            });
        }

        [HttpPost("{id}")]
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


            if (user.UserName == user.Email)
            {
                user.UserName = model.NewEmail;
            }

            user.Email = model.NewEmail;
            user.EmailConfirmed = false;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new KnownException(string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));

            return Ok(new FormSubmitResponse<UpdateEmailFormModel>
            {
                Snack = await Service<ITranslationsRepository>().GetValueOrSlug("updated"),
                SnackType = "success",
                SnackDuration = 3000
            });
        }


        [HttpPost("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> UpdateUserName([FromRoute] string id,
            [Required] [FromBody] UpdateUserNameFormModel model)
        {
            model.NewUserName = model.NewUserName.Trim().ToLower();
            if (model.OldUserName == model.NewUserName)
            {
                throw new KnownException("The new username is the same as old username.");
            }

            var userManager = Service<UserManager<User>>();
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (user.UserName != model.OldUserName)
            {
                throw new KnownException("Old username is not the same. Please try again.");
            }

            user.UserName = model.NewUserName;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new KnownException(string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));

            return Ok(new FormSubmitResponse<UpdateEmailFormModel>
            {
                Snack = await Service<ITranslationsRepository>().GetValueOrSlug("updated"),
                SnackType = "success",
                SnackDuration = 3000
            });
        }

        [HttpPost("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> UpdateUserProfile([FromRoute] string id,
            [Required] [FromBody] UpdateUserProfileFormModel model)
        {
            var userManager = Service<UserManager<User>>();
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();


            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            await userManager.UpdateAsync(user);

            return Ok(new FormSubmitResponse<UpdateEmailFormModel>
            {
                Snack = await Service<ITranslationsRepository>().GetValueOrSlug("updated"),
                SnackType = "success",
                SnackDuration = 3000
            });
        }


        [HttpPost("{id}")]
        public virtual async Task<ActionResult<UserViewModel>> ConfirmEmail([FromRoute] string id)
        {
            var user = await Repo.GetOneOrThrow(id);
            user.EmailConfirmed = true;
            await Repo.SaveChanges();
            return Ok();
        }

        [HttpPost("{id}")]
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
                // .Where(role => role.Name != "God")
                .ToListAsync();
            var currentUserRank = UserFromClaims.Roles
                .Select(name => roles.FirstOrDefault(role => role.Name == name))
                .Where(role => role != null)
                .Min(role => role.Rank);

            var visibleRoles = roles.Where(role => role.Rank >= currentUserRank)
                .OrderBy(role => role.Rank)
                .ThenBy(role => role.Name);
            return visibleRoles.Select(role => new ValueLabelModel
                {
                    Value = role.Name,
                    Label = role.Name,
                    Description = role.Description
                })
                .ToList();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (UserFromClaims.Id == id)
            {
                throw new KnownException("Can't delete your own user.");
            }

            var usersManager = Service<UserManager<User>>();
            var user = await usersManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new KnownException("User not found", 404);
            }

            await usersManager.DeleteAsync(user);
            return Ok();
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