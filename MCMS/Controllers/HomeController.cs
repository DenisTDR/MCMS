using System.Diagnostics;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MCMS.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace MCMS.Controllers
{
    public class HomeController : AdminUiController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AdminRoute("/")]
        public override Task<IActionResult> Index()
        {
            return Task.FromResult(View() as IActionResult);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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