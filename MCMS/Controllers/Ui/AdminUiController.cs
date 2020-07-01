using System;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Ui
{
    [AdminRoute("[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public abstract class AdminUiController : Controller
    {
        public IServiceProvider ServiceProvider => HttpContext.RequestServices;
        protected bool UsesModals { get; set; }

        [HttpGet]
        [AdminRoute("/[controller]")]
        public abstract Task<IActionResult> Index();

        protected Task<IActionResult> CustomIndex()
        {
            return Task.FromResult(View() as IActionResult);
        }

        protected IActionResult RedirectBackOrOk()
        {
            if (UsesModals)
            {
                return Ok();
            }

            if (HttpContext.Request.Query.ContainsKey("returnUrl"))
            {
                var returnUrl = HttpContext.Request.Query["returnUrl"];
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}