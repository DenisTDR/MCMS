using System;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using MCMS.Base.Auth;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using MCMS.Base.Extensions;

namespace MCMS.Base.Controllers
{
    [Route("~/[controller]/[action]")]
    public class BaseController : Controller
    {
        protected IServiceProvider ServiceProvider => HttpContext.RequestServices;
        protected IServiceProvider Services => ServiceProvider;
        protected IMapper Mapper => Service<IMapper>();

        private UserWithRoles _user;

        protected virtual UserWithRoles UserFromClaims =>
            _user ??= new UserWithRoles
            {
                Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User.FindFirstValue(ClaimTypes.Email),
                Roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList()
            };

        protected T Service<T>() => Services.Service<T>();
        protected IRepository<T> Repo<T>() where T : class, IEntity => Services.Repo<T>();
    }
}