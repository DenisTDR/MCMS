using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MCMS.Builder;
using MCMS.Data;
using MCMS.Filters;
using MCMS.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace MCMS
{
    public class MBaseSpecifications : MSpecifications
    {
        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            AddCorsFromEnv(app);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        public override void ConfigMvc(MvcOptions options)
        {
            options.Filters.Add<CustomExceptionFilter>();
            options.Filters.Add<LayoutFilter>();
            
            // won't use this because of reasons
            // options.Conventions.Add(new ApiControllerNameAttributeConvention());
        }

        private void AddCorsFromEnv(IApplicationBuilder app)
        {
            var corsHostsStr = Env.Get("ALLOWED_CORS_HOSTS");
            if (string.IsNullOrEmpty(corsHostsStr))
            {
                return;
            }

            var corsHosts = corsHostsStr.Split(';');
            app.UseCors(builder =>
            {
                foreach (var corsHost in corsHosts)
                {
                    if (corsHost == "*")
                    {
                        builder = builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    }
                    else
                    {
                        builder = builder.WithOrigins(corsHost.Trim()).AllowAnyHeader();
                    }
                }
            });
        }
    }
}