using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MCMS.Base.Auth;
using MCMS.Base.Data;
using MCMS.Logging.AuditLogEntries;
using MCMS.Logging.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Logging
{
    public class MAuditLogger<TCategoryName> : IMAuditLogger<TCategoryName>
    {
        private readonly IRepository<User> _usersRepo;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MAuditLogger(
            IHttpContextAccessor httpContextAccessor,
            IRepository<User> usersRepo,
            IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _usersRepo = usersRepo;
            _serviceProvider = serviceProvider;
        }

        public void Log(Dictionary<string, object> data = null)
        {
            var httpContext = _httpContextAccessor.HttpContext ??
                              throw new Exception("Got null HttpContext in MAuditLogger.");

            var actionName = httpContext.Request.RouteValues["action"]?.ToString();
            var controllerName = httpContext.Request.RouteValues["controller"]?.ToString();

            var log = new AuditLogEntryEntity
            {
                Author = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) is { } userId
                    ? new User {Id = userId}
                    : null,
                Ip = httpContext?.Connection.RemoteIpAddress?.ToString(),
                TraceIdentifier = httpContext.TraceIdentifier,
                Action = actionName,
                Controller = controllerName,
                Category = typeof(TCategoryName).Name,
                Path = httpContext.Request.Path,
                Data = data,
                Begin = DateTime.Now,
            };
            var worker = _serviceProvider.GetRequiredService<MAuditLogWorker>();
            worker.Enqueue(new LogActionWrapper<AuditLogEntryEntity>(log));
        }

        public void UpdateLog(Dictionary<string, object> data = null)
        {
            var httpContext = _httpContextAccessor.HttpContext ??
                              throw new Exception("Got null HttpContext in MAuditLogger.");
            var log = new AuditLogEntryEntity
            {
                TraceIdentifier = httpContext.TraceIdentifier,
                Data = data,
                End = DateTime.Now,
            };
            var worker = _serviceProvider.GetRequiredService<MAuditLogWorker>();
            worker.Enqueue(new LogActionWrapper<AuditLogEntryEntity>(log, ActionType.Update));
        }
    }
}