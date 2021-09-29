using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Auth.Models;
using MCMS.Auth.Session;
using MCMS.Auth.Tokens.Dtos;
using MCMS.Auth.Tokens.Models;
using MCMS.Base.Attributes;
using MCMS.Base.Auth;
using MCMS.Base.Exceptions;
using MCMS.Base.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Auth.Controllers
{
    [Authorize]
    public abstract class AuthBaseController<TLogin> : ApiController where TLogin : LoginRequestFormModel
    {
        protected UserManager<User> UserManager => ServiceProvider.GetRequiredService<UserManager<User>>();
        protected SignInManager<User> SignInManager => ServiceProvider.GetRequiredService<SignInManager<User>>();
        private ISessionService SessionService => ServiceProvider.GetRequiredService<ISessionService>();

        protected IEnumerable<IMAuthInterceptor> AuthInterceptors =>
            ServiceProvider.GetRequiredService<IEnumerable<IMAuthInterceptor>>();

        [HttpPost]
        [ModelValidation]
        [AllowAnonymous]
        public virtual async Task<ActionResult<SessionDto>> Login([FromBody] [Required] TLogin model)
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

            var session = await SessionService.CreateSession(user, IpAddress());

            return Ok(session);
        }

        [HttpPost]
        [ModelValidation]
        [AllowAnonymous]
        [CustomExceptionFilter(typeof(DbUpdateConcurrencyException),
            "This action was already completed through another request.")]
        public async Task<IActionResult> RefreshToken([FromBody] [Required] TokenRequestDto model)
        {
            var newSession = await SessionService.RefreshSession(model.Token, IpAddress());
            return Ok(newSession);
        }

        [HttpPost]
        [ModelValidation]
        public async Task<IActionResult> RevokeToken([FromBody] [Required] TokenRequestDto model)
        {
            await SessionService.RevokeRefreshToken(model.Token, IpAddress());
            return Ok();
        }


        [HttpGet]
        public async Task<ActionResult<List<RefreshTokenEntity>>> GetRefreshTokens()
        {
            var tokens = await SessionService.RefreshTokensService.GetAllRefreshTokensForUser(UserFromClaims);
            return Ok(tokens.OrderByDescending(r => r.Created));
        }

        [HttpGet]
        public virtual IActionResult IsAuthorized()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }

        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public virtual IActionResult IsAuthorizedModerator()
        {
            return IsAuthorized();
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }
    }
}