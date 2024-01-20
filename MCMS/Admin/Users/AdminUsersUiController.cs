using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MCMS.Admin.Users.Models;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Base.Data;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Controllers.Ui;
using MCMS.Display.DetailsConfig;
using MCMS.Display.ModelDisplay;
using MCMS.Display.TableConfig;
using MCMS.SwaggerFormly;
using MCMS.SwaggerFormly.FormParamsHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MCMS.Admin.Users
{
    [Authorize(Roles = "Admin, Moderator")]
    public class AdminUsersUiController : AdminUiController
    {
        protected IRepository<User> Repo => ServiceProvider.GetRepo<User>();

        protected ITableConfigService TableConfigService =>
            Service<UsersTableConfigService>();

        protected IDetailsConfigServiceT<UserViewModel> DetailsConfigService =>
            Service<IDetailsConfigServiceT<UserViewModel>>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.ModelName = EntityHelper.GetEntityName<User>();
        }

        public override async Task<IActionResult> Index()
        {
            TableConfigService.ServerSide = true;
            return View("BasicPages/Index", await GetIndexPageConfig());
        }

        [HttpGet]
        [Route("{id}")]
        [ViewLayout("_ModalLayout")]
        public async Task<IActionResult> Details([FromRoute] string id)
        {
            var userVm = await GetUserWithRoles(id);
            return View(DetailsConfigService.Wrap(userVm));
        }


        [HttpGet]
        [Route("{id}")]
        [ViewLayout("_ModalLayout")]
        public async Task<IActionResult> ConfirmEmail([FromRoute] string id)
        {
            var user = await Repo.GetOneOrThrow(id);
            var userVm = Mapper.Map<UserViewModel>(user);
            return View(userVm);
        }


        [HttpGet]
        [Route("{id}")]
        [ViewLayout("_ModalLayout")]
        public async Task<IActionResult> ResendActivationMail([FromRoute] string id)
        {
            var user = await Repo.GetOneOrThrow(id);
            var userVm = Mapper.Map<UserViewModel>(user);
            return View(userVm);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            return View("BasicModals/DeleteModal", e);
        }

        [HttpDelete("{id}"), ActionName("Delete")]
        [Produces("application/json")]
        public virtual async Task<IActionResult> DeleteConfirmed([FromRoute] string id)
        {
            var isCurrentUser = User.FindFirstValue("Id") == id;
            if (isCurrentUser)
            {
                return BadRequest("Can't delete your own user.");
            }

            var usersManager = Service<UserManager<User>>();
            var user = await usersManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await usersManager.DeleteAsync(user);
            return Ok();
        }

        private async Task<UserViewModel> GetUserWithRoles(string id)
        {
            var user = await Repo.GetOneOrThrow(id);
            var userVm = Mapper.Map<UserViewModel>(user);
            userVm.RolesList = (await Service<UserManager<User>>().GetRolesAsync(user))
                .ToList();
            return userVm;
        }

        [NonAction]
        public virtual async Task<IndexPageConfig> GetIndexPageConfig()
        {
            return new()
            {
                IndexPageTitle = "Users",
                TableConfig = await TableConfigService.GetTableConfig()
            };
        }

        public IActionResult Create()
        {
            ViewBag.FormParamsService =
                new FormParamsService(Url, TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController)),
                    nameof(CreateUserFormModel));
            ViewBag.ModalDialogClasses = "modal-lg";
            return View("BasicModals/CreateModal");
        }

        [HttpGet]
        [Route("{id}")]
        [ViewLayout("_ModalLayout")]
        public async Task<IActionResult> UpdateRoles([FromRoute] string id)
        {
            var userVm = await GetUserWithRoles(id);
            var fps =
                new FormParamsService(Url, TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController)),
                    nameof(UpdateRolesFormModel));


            var fp = fps.ForCreate();

            fp.SubmitUrl = Url.ActionLink(nameof(AdminUsersAdminApiController.UpdateRoles),
                TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController)), new { id = userVm.Id });

            fp.HideSubmitButton();
            fp.UseSpinnerOuterOverlay();
            fp.AdditionalFields = new { roles = userVm.RolesList };

            // ViewBag.ModalDialogClasses = "modal-md";

            return View((userVm.FullName, fp));
        }

        [HttpGet]
        [Route("{id}")]
        [ViewLayout("_ModalLayout")]
        public async Task<IActionResult> UpdateEmail([FromRoute] string id)
        {
            var userVm = await GetUserWithRoles(id);
            var fps =
                new FormParamsService(Url, TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController)),
                    nameof(UpdateEmailFormModel));


            var fp = fps.ForCreate();

            fp.SubmitUrl = Url.ActionLink(nameof(AdminUsersAdminApiController.UpdateEmail),
                TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController)), new { id = userVm.Id });

            fp.HideSubmitButton();
            fp.UseSpinnerOuterOverlay();
            fp.AdditionalFields = new { oldEmail = userVm.Email };

            // ViewBag.ModalDialogClasses = "modal-sm";

            return View((userVm.FullName, fp));
        }
        [HttpGet]
        [Route("{id}")]
        [ViewLayout("_ModalLayout")]
        public async Task<IActionResult> UpdateUserProfile([FromRoute] string id)
        {
            var user = await Repo.GetOneOrThrow(id);
            var fps =
                new FormParamsService(Url, TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController)),
                    nameof(UpdateUserProfileFormModel));


            var fp = fps.ForCreate();

            fp.SubmitUrl = Url.ActionLink(nameof(AdminUsersAdminApiController.UpdateUserProfile),
                TypeHelpers.GetControllerName(typeof(AdminUsersAdminApiController)), new { id = user.Id });

            fp.HideSubmitButton();
            fp.UseSpinnerOuterOverlay();
            fp.AdditionalFields = new { firstName = user.FirstName, lastName = user.LastName, phoneNumber = user.PhoneNumber };

            return View((user.FullName, fp));
        }
    }
}