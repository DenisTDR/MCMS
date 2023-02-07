using System;
using MCMS.Base.Builder;
using MCMS.Base.Extensions;
using MCMS.Logging.AuditLogs;
using MCMS.Logging.AuditLogs.Worker;
using MCMS.Logging.Logs.LogEntries;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MCMS.Logging
{
    public class MLoggingSpecifications : MSpecifications
    {
        public MLoggingSpecifications()
        {
            HasRazorViews = true;
            PrePublishRootPath = "../MCMS";
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddOptions<LayoutIncludesOptions>().Configure(c => { c.AddForPages("MLoggingIncludes"); });

            services.AddScoped(typeof(IMAuditLogger<>), typeof(MAuditLogger<>));

            services.AddHttpContextAccessor();

            services.AddSingleton<MAuditLogWorker>();
            
            services.AddScoped<LoggerService>();
        }

        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var auditLogWorker = serviceProvider.Service<MAuditLogWorker>();
            auditLogWorker.Start();

            serviceProvider.Service<IHostApplicationLifetime>()
                .ApplicationStopping.Register(() => { auditLogWorker.Stop(); });
        }
    }
}