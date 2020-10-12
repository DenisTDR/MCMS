using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MCMS.Base.Builder;
using MCMS.Base.Helpers;
using MCMS.Builder.Helpers;
using MCMS.Data;
using MCMS.Data.Seeder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MCMS.Builder
{
    public class MApp
    {
        public IEnumerable<MSpecifications> Specifications => _specifications.ToList();
        private readonly IList<MSpecifications> _specifications;
        private readonly Action<IServiceCollection> _addDbContextAction;
        public IWebHostEnvironment HostEnvironment { get; }

        public MApp(IWebHostEnvironment hostEnvironment, IList<MSpecifications> specifications,
            Action<IServiceCollection> addDbContextAction)
        {
            HostEnvironment = hostEnvironment;
            _specifications = specifications;
            _addDbContextAction = addDbContextAction;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var mvcBuilder = services
                .AddMvc(options =>
                {
                    // options.Conventions.Insert(0, new MRouteConvention(Env.Get("ADMIN_ROUTE_PREFIX")));
                    foreach (var mSpecifications in _specifications)
                    {
                        mSpecifications.ConfigMvc(options);
                    }
                })
                .AddNewtonsoftJson(mvcJsonOptions =>
                {
                    mvcJsonOptions.SerializerSettings.Converters.Add(
                        new Newtonsoft.Json.Converters.StringEnumConverter());
                    mvcJsonOptions.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            mvcBuilder = _specifications.Aggregate(mvcBuilder,
                (current, mSpecifications) => mSpecifications.MvcChain(current));

            if (HostEnvironment.IsDevelopment() && Env.GetBool("RAZOR_RUNTIME_COMPILATION"))
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

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseForwardedHeaders();
            var logger = serviceProvider.GetService<ILogger<MApp>>();
            if (HostEnvironment.IsDevelopment())
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

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");
            app.Use(async (ctx, next) =>
            {
                if (ctx.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
                {
                    var statusCodeFeature = ctx.Features.Get<IStatusCodePagesFeature>();

                    if (statusCodeFeature != null && statusCodeFeature.Enabled)
                        statusCodeFeature.Enabled = false;
                }

                await next();
            });


            app.UseStaticFiles();

            app.UseRouting();

            foreach (var mSpec in _specifications)
            {
                mSpec.Configure(app, serviceProvider);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            if (Env.GetBool("MIGRATE_ON_START"))
            {
                logger.LogInformation("Checking pending migrations.");
                var dbContext = serviceProvider.GetService<BaseDbContext>();
                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    logger.LogWarning("Migrating database...");
                    dbContext.Database.Migrate();
                }
            }

            if (Env.GetBool("SEED_ON_START"))
            {
                serviceProvider.GetService<DataSeeder>().SeedFromFile().Wait();
            }

            // assert that variable is set correctly
            if (!(Env.GetOrThrow("EXTERNAL_URL") is { } url) || url.EndsWith('/') || !url.Contains("http"))
            {
                Utils.DieWith("EXTERNAL_URL must include protocol and must not end with /");
            }
        }
    }
}