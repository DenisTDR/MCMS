using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Ui
{
    [Route("[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class UiController : Controller
    {
        public IServiceProvider ServiceProvider => HttpContext.RequestServices;
        protected bool UsesModals { get; set; }

        [HttpGet]
        public abstract Task<IActionResult> Index();

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