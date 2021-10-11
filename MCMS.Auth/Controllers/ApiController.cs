using System.Linq;
using System.Security.Claims;
using MCMS.Auth.Jwt;
using MCMS.Base.Auth;
using MCMS.Base.Controllers.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace MCMS.Auth.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class ApiController : BaseApiController
    {
        private UserWithRoles _user;

        protected override UserWithRoles UserFromClaims =>
            _user ??= new UserWithRoles
            {
                Id = User.FindFirstValue(Claims.Id),
                Email = User.FindFirstValue(Claims.Username),
                Roles = User.FindAll(Claims.Role).Select(claim => claim.Value).ToList()
            };
    }
}