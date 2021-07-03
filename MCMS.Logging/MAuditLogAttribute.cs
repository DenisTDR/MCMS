using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Logging
{
    public class MAuditLogAttribute : ActionFilterAttribute
    {
        private IMAuditLogger<MAuditLogAttribute> _auditLogger;


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _auditLogger = context.HttpContext.RequestServices.GetRequiredService<IMAuditLogger<MAuditLogAttribute>>();
            var data = new Dictionary<string, object>
            {
                {"method", context.HttpContext.Request.Method}, {"actionArgument", context.ActionArguments}
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