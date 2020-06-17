using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using MCMS.Builder.Helpers;
using MCMS.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace MCMS.Builder
{
    public class MApp
    {
        public IEnumerable<MSpecifications> Specifications => _specifications.ToList();
        private readonly IList<MSpecifications> _specifications;
        private readonly Action<IServiceCollection> _addDbContextAction;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MApp(IWebHostEnvironment hostEnvironment, IList<MSpecifications> specifications,
            Action<IServiceCollection> addDbContextAction)
        {
            _hostEnvironment = hostEnvironment;
            _specifications = specifications;
            _addDbContextAction = addDbContextAction;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            RegisterViewsPathsFromEnvVar(services);
            var mvcBuilder = services
                .AddMvc()
                .AddNewtonsoftJson(mvcJsonOptions =>
                {
                    mvcJsonOptions.SerializerSettings.Converters.Add(
                        new Newtonsoft.Json.Converters.StringEnumConverter());
                    mvcJsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
            // var mvcBuilder = services.AddRazorPages();
            if (_hostEnvironment.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            new MAppEntitiesHelper(this).ProcessSpecifications(services);

            _addDbContextAction(services);

            services.AddAutoMapper(expression =>
            {
                foreach (var buildMappingConfig in new MAppMappingHelper(this).BuildMappingConfigs())
                {
                    buildMappingConfig.CreateMaps(expression);
                }
            }, typeof(MBaseSpecifications));

            foreach (var smpSpec in _specifications)
            {
                smpSpec.ConfigureServices(services);
            }
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            if (_hostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            RegisterWwwrootPaths(app);

            app.UseStaticFiles();

            app.UseRouting();

            foreach (var rmpSpec in _specifications)
            {
                rmpSpec.Configure(app, serviceProvider);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            // assert that variable is set correctly
            if (!(Env.GetOrThrow("EXTERNAL_URL") is { } url) || url.EndsWith('/') || !url.Contains("http"))
            {
                Utilis.DieWith("EXTERNAL_URL must include protocol and must not end with /");
            }
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