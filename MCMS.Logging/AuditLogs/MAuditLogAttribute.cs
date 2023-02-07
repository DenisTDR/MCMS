using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MCMS.Logging.AuditLogs
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
            if (ShouldSkipLog(context))
                return;

            _auditLogger = context.HttpContext.RequestServices.Service<IMAuditLogger<MAuditLogAttribute>>();
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
            if (ShouldSkipLog(context))
                return;

            var data = new Dictionary<string, object> {["statusCode"] = context.HttpContext.Response.StatusCode};
            _auditLogger.UpdateLog(data);
        }

        private static readonly string[] NonReadonlyHttpMethods = {"POST", "PUT", "DELETE", "PATCH"};

        private bool ShouldSkipLog(FilterContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor
                && controllerActionDescriptor.MethodInfo.GetCustomAttributes<ReadOnlyApiAttribute>().FirstOrDefault()
                    ?.IsReadOnly == true)
            {
                return true;
            }

            return !IgnoreReadOnlyRequests || !NonReadonlyHttpMethods.Contains(context.HttpContext.Request.Method);
        }
    }
}