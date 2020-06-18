using System;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers
{
    public class UiController : Controller
    {
        public IServiceProvider ServiceProvider => HttpContext.RequestServices;
    }
}