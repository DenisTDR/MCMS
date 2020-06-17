using System;
using AutoMapper;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MCMS.Builder;
using MCMS.Builder.Helpers;
using MCMS.Data;
using MCMS.Helpers;

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