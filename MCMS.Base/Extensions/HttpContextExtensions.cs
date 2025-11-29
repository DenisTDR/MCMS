using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Base.Extensions;

public static class HttpContextExtensions
{
    extension(HttpContext httpContext)
    {
        public ActionContext GetActionContext()
        {
            var endpoint = httpContext.GetEndpoint();
            var actionDescriptor =
                endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>() ??
                new ActionDescriptor();

            return new ActionContext(
                httpContext,
                httpContext.GetRouteData(),
                actionDescriptor
            );
        }

        public IUrlHelper GetUrlHelper()
        {
            var factory = httpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();

            return factory.GetUrlHelper(httpContext.GetActionContext());
        }
    }
}