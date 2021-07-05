using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Logging
{
    public class MAuditLogAttribute : ActionFilterAttribute
    {
        private bool IgnoreReadOnlyRequests { get; }

        public MAuditLogAttribute(bool ignoreReadOnlyRequests = true)
        {
            IgnoreReadOnlyRequests = ignoreReadOnlyRequests;
        }

        private IMAuditLogger<MAuditLogAttribute> _auditLogger;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (next == null)
                throw new ArgumentNullException(nameof(next));

            OnActionExecuting(context);
            if (context.Result == null)
            {
                OnActionExecuted(await next());
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (ShouldSkipLog(context.HttpContext.Request))
                return;

            _auditLogger = context.HttpContext.RequestServices.GetRequiredService<IMAuditLogger<MAuditLogAttribute>>();
            var request = context.HttpContext.Request;
            var data = new Dictionary<string, object>
            {
                {"method", request.Method}
            };
            if (context.ActionArguments.Any())
            {
                data["actionArguments"] = context.ActionArguments;
            }

            _auditLogger.Log(data);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (ShouldSkipLog(context.HttpContext.Request))
                return;

            var data = new Dictionary<string, object> {["statusCode"] = context.HttpContext.Response.StatusCode};
            _auditLogger.UpdateLog(data);
        }

        private static readonly string[] NonReadonlyHttpMethods = {"POST", "PUT", "DELETE", "PATCH"};

        private bool ShouldSkipLog(HttpRequest request)
        {
            return !IgnoreReadOnlyRequests || NonReadonlyHttpMethods.Contains(request.Method);
        }
    }
}