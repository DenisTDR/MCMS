using System;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers
{
    [Route("[controller]/[action]")]
    public class UiController : Controller
    {
        public IServiceProvider ServiceProvider => HttpContext.RequestServices;
    }
}