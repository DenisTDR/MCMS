using System;
using MCMS.Base.Builder;
using MCMS.Logging.Worker;
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
        }

        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var auditLogWorker = serviceProvider.GetRequiredService<MAuditLogWorker>();
            auditLogWorker.Start();

            var appLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
            appLifetime.ApplicationStopping.Register(() => { auditLogWorker.Stop(); });
        }
    }
}