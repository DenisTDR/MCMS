using System;
using System.Security.Claims;
using AutoMapper;
using MCMS.Base.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers
{
    [Route("[controller]/[action]")]
    public class BaseController : Controller
    {
        protected IServiceProvider ServiceProvider => HttpContext.RequestServices;
        protected IMapper Mapper => ServiceProvider.GetService<IMapper>();

        private User _user;

        protected User UserFromClaims =>
            _user ??= new User
            {
                Id = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Email = User.FindFirst(ClaimTypes.Email).Value,
            };
    }
}