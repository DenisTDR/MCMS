using System;
using AutoMapper;
using MCMS.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers.Api
{
    [ApiRoute("[controller]/[action]")]
    [Produces("application/json")]
    [Authorize]
    public class ApiController : Controller
    {
        public IServiceProvider ServiceProvider => HttpContext.RequestServices;

        public IMapper Mapper => ServiceProvider.GetService<IMapper>();
    }
}