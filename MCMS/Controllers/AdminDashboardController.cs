using System.Threading.Tasks;
using MCMS.Base.Auth;
using MCMS.Controllers.Ui;
using MCMS.Data.Seeder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : AdminUiController
    {
        public override Task<IActionResult> Index()
        {
            // return View();
            return Task.FromResult(View() as IActionResult);
        }

        public IActionResult Seed()
        {
            return View();
        }

        [HttpPost, ActionName("Seed")]
        public async Task<IActionResult> BuildSeed()
        {
            var dataSeeder = ServiceProvider.GetService<DataSeeder>();
            var seedData = await dataSeeder.BuildSeed();
            return View(seedData);
        }

        [HttpPost]
        public async Task<IActionResult> SeedRoles()
        {
            var roleManager = ServiceProvider.GetService<RoleManager<Role>>();
            if (!await roleManager.Roles.AnyAsync(r => r.Name == "Admin"))
            {
                await roleManager.CreateAsync(new Role {Name = "Admin"});
            }

            return RedirectToAction(nameof(Seed));
        }
    }
}