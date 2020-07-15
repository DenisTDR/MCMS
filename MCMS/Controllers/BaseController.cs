using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers
{
    [Route("[controller]/[action]")]
    public class BaseController : Controller
    {
        protected IServiceProvider ServiceProvider => HttpContext.RequestServices;
        protected IMapper Mapper => ServiceProvider.GetService<IMapper>();
    }
}