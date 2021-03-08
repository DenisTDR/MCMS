using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Auth.Jwt;
using MCMS.Auth.Models;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Base.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Auth.Controllers
{
    [Authorize]
    public abstract class AuthBaseController<TLogin> : ApiController where TLogin : LoginRequestFormModel
    {
        protected UserManager<User> UserManager => ServiceProvider.GetRequiredService<UserManager<User>>();
        protected SignInManager<User> SignInManager => ServiceProvider.GetRequiredService<SignInManager<User>>();
        protected IJwtFactory JwtFactory => ServiceProvider.GetRequiredService<IJwtFactory>();

        protected IEnumerable<IMAuthInterceptor> AuthInterceptors =>
            ServiceProvider.GetRequiredService<IEnumerable<IMAuthInterceptor>>();

        [HttpPost]
        [ModelValidation]
        [AllowAnonymous]
        public virtual async Task<IActionResult> Login([FromBody] [Required] TLogin model)
        {
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                throw new KnownException("Invalid credentials");
            }

            var result = await SignInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                throw new KnownException("Invalid credentials");
            }

            foreach (var mAuthInterceptor in AuthInterceptors)
            {
                result = await mAuthInterceptor.OnSignIn(user, result, SignInType.Api);
            }

            if (result.IsNotAllowed)
            {
                throw new KnownException("You are not allowed to sign in here");
            }

            var roles = await UserManager.GetRolesAsync(user);
            var session = JwtFactory.GenerateSession(user, roles, user.Id);

            return Ok(session);
        }

        [HttpGet]
        public virtual IActionResult IsAuthorized()
        {
            var claims = User.Claims.Select(c => new {c.Type, c.Value});
            return Ok(claims);
        }

        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public virtual IActionResult IsAuthorizedModerator()
        {
            return IsAuthorized();
        }
    }
}