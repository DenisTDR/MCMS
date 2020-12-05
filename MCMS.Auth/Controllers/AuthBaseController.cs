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
    public abstract class AuthBaseController<TLogin> : JwtApiController where TLogin : LoginRequestModel
    {
        protected UserManager<User> UserManager => ServiceProvider.GetRequiredService<UserManager<User>>();
        protected SignInManager<User> SignInManager => ServiceProvider.GetRequiredService<SignInManager<User>>();
        protected IJwtFactory JwtFactory => ServiceProvider.GetRequiredService<IJwtFactory>();

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

            var roles = await UserManager.GetRolesAsync(user);
            var session = JwtFactory.GenerateSession(user.UserName, roles, user.Id);

            return Ok(session);
        }

        [HttpGet]
        [Authorize]
        public virtual IActionResult IsAuthorized()
        {
            var claims = User.Claims.Select(c =>
                new
                {
                    Type = c.Type,
                    Value = c.Value
                });
            return Ok(claims);
        }

        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public virtual IActionResult IsAuthorizedUser()
        {
            return IsAuthorized();
        }
    }
}