using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Logging
{
    public class MAuditLogAttribute : ActionFilterAttribute
    {
        private IMAuditLogger<MAuditLogAttribute> _auditLogger;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (next == null)
                throw new ArgumentNullException(nameof(next));

            await OnActionExecutingAsync(context);
            if (context.Result == null)
            {
                OnActionExecuted(await next());
            }
        }

        public async Task OnActionExecutingAsync(ActionExecutingContext context)
        {
            _auditLogger = context.HttpContext.RequestServices.GetRequiredService<IMAuditLogger<MAuditLogAttribute>>();
            var request = context.HttpContext.Request;
            var data = new Dictionary<string, object>
            {
                {"actionArguments", context.ActionArguments}, {"method", request.Method}
            };

            _auditLogger.Log(data);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var data = new Dictionary<string, object> {["statusCode"] = context.HttpContext.Response.StatusCode};
            _auditLogger.UpdateLog(data);
        }
    }
}