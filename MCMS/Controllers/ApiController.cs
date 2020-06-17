using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [Authorize]
    public class ApiController : ControllerBase
    {
        public IServiceProvider ServiceProvider => HttpContext.RequestServices;

        public IMapper Mapper => ServiceProvider.GetService<IMapper>();
    }
}