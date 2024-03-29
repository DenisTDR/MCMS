using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCMS.Auth.Identity
{
    internal class ExternalLoginsPageFilter<TUser> : IAsyncPageFilter where TUser : class
    {
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var result = await next();
            if (result.Result is PageResult page)
            {
                var signInManager = context.HttpContext.RequestServices.Service<SignInManager<TUser>>();
                var schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
                var hasExternalLogins = schemes.Any();

                page.ViewData["ManageNav.HasExternalLogins"] = hasExternalLogins;
            }
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}