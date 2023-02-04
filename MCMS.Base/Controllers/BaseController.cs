using System;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using MCMS.Base.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Base.Controllers
{
    [Route("~/[controller]/[action]")]
    public class BaseController : Controller
    {
        protected IServiceProvider ServiceProvider => HttpContext.RequestServices;
        protected IServiceProvider Services => ServiceProvider;
        protected IMapper Mapper => ServiceProvider.GetRequiredService<IMapper>();

        private UserWithRoles _user;

        protected virtual UserWithRoles UserFromClaims =>
            _user ??= new UserWithRoles
            {
                Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User.FindFirstValue(ClaimTypes.Email),
                Roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList()
            };
    }
}