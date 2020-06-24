using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MCMS.Builder;
using MCMS.Data;
using MCMS.Data.Seeder;
using MCMS.Display.ModelDisplay;
using MCMS.Filters;
using MCMS.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;

namespace MCMS
{
    public class MBaseSpecifications : MSpecifications
    {
        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            AddCorsFromEnv(app);
            RegisterWwwrootPaths(app);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            RegisterViewsPathsFromEnvVar(services);
            services.AddCors();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<DataSeeder>();
            services.AddOptions<EntitySeeders>().Configure(seeders => { seeders.Add<RoleSeeder>(); });
            services.AddScoped(typeof(ModelDisplayConfigForControllerService<,,,,>));
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

        #region VIEWS_AND_WWW_PATHS

        public void RegisterViewsPathsFromEnvVar(IServiceCollection services)
        {
            foreach (var s in Env.GetArray("VIEWS_AND_WWW_PATHS"))
            {
                var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), s));
                var fileProvider = new PhysicalFileProvider(path);
                services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
                {
                    options.FileProviders.Add(fileProvider);
                });
            }
        }

        public void RegisterWwwrootPaths(IApplicationBuilder app)
        {
            foreach (var s in Env.GetArray("VIEWS_AND_WWW_PATHS"))
            {
                var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), s, "wwwroot"));
                if (Directory.Exists(path))
                {
                    app.UseStaticFiles(new StaticFileOptions
                        {FileProvider = new PhysicalFileProvider(path), RequestPath = ""});
                }
            }
        }

        #endregion
    }
}