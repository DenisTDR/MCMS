using System.Diagnostics;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;
using MCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;

namespace MCMS.Controllers
{
    [AllowAnonymous]
    public class HomeController : AdminUiController
    {
        [AdminRoute("~/")]
        public override Task<IActionResult> Index()
        {
            return Task.FromResult(View() as IActionResult);
        }

        [AdminRoute("~/health")]
        public IActionResult Health()
        {
            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AdminRoute("~/Error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Exception = exceptionHandlerPathFeature?.Error,
            };

            if (int.TryParse(Request.Query["code"], out var statusCode))
            {
                errorModel.StatusCode = statusCode;
            }

            return View(errorModel);
        }
    }
}