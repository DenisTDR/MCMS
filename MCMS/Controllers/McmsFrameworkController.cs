using MCMS.Controllers.Ui;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers
{
    public class McmsFrameworkController : UiController
    {
        public IActionResult FrameworkInfo()
        {
            return View();
        }
    }
}