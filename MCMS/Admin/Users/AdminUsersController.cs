using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Base.Helpers;
using MCMS.Controllers.Ui;
using MCMS.Data;
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Admin.Users
{
    [Authorize(Roles = "Admin, Moderator")]
    public class AdminUsersController : AdminUiController
    {
        protected IRepository<User> Repo => ServiceProvider.GetService<IRepository<User>>();

        protected IModelDisplayConfigService ModelDisplayConfigService =>
            ServiceProvider.GetService<UsersTableModelDisplayConfigService>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.ModelName = EntityHelper.GetEntityName<User>();
            ViewBag.ModelDisplayConfigService = ModelDisplayConfigService;
            ViewBag.UsesModals = UsesModals;
        }

        public override async Task<IActionResult> Index()
        {
            return View("BasicPages/Index", await ModelDisplayConfigService.GetTableConfig(Url, ViewBag, false));
        }

        [HttpGet]
        [Route("{id}")]
        [ViewLayout("_ModalLayout")]
        public async Task<IActionResult> Details([FromRoute] string id)
        {
            var userVm = await GetUserWithRoles(id);
            ViewBag.Fields = ModelDisplayConfigService.GetDetailsFields();
            return View(userVm);
        }

        [HttpGet]
        [Route("{id}")]
        [ViewLayout("_ModalLayout")]
        public async Task<IActionResult> ChangeRoles([FromRoute] string id)
        {
            var userVm = await GetUserWithRoles(id);
            ViewBag.Roles = await ServiceProvider.GetService<RoleManager<Role>>().Roles.Select(role => role.Name)
                .ToListAsync();
            return View(userVm);
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

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            return View("BasicModals/DeleteModal", e);
        }

        [HttpPost("{id}"), ActionName("Delete")]
        [Produces("application/json")]
        public virtual async Task<IActionResult> DeleteConfirmed([FromRoute] string id)
        {
            var isCurrentUser = User.FindFirstValue("Id") == id;
            if (isCurrentUser)
            {
                return BadRequest("Can't delete your own user.");
            }

            var usersManager = ServiceProvider.GetService<UserManager<User>>();
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
            userVm.RolesList = (await ServiceProvider.GetService<UserManager<User>>().GetRolesAsync(user)).ToList();
            return userVm;
        }
    }
}