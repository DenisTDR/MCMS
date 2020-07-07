using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Controllers.Ui;
using MCMS.Data;
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : AdminUiController
    {
        protected IRepository<User> Repo => ServiceProvider.GetService<IRepository<User>>();
        protected IModelDisplayConfigService ModelDisplayConfigService => new UsersTableModelDisplayConfigService();

#pragma warning disable 1998
        public override async Task<IActionResult> Index()
#pragma warning restore 1998
        {
            return View("BasicPages/Index", ModelDisplayConfigService.GetTableConfig(Url, ViewBag, false));
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

        private async Task<UserViewModel> GetUserWithRoles(string id)
        {
            var user = await Repo.GetOneOrThrow(id);
            var userVm = Mapper.Map<UserViewModel>(user);
            userVm.RolesList = (await ServiceProvider.GetService<UserManager<User>>().GetRolesAsync(user)).ToList();
            return userVm;
        }
    }
}