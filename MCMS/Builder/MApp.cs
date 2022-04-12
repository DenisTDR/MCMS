using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MCMS.Admin;
using MCMS.Base.Builder;
using MCMS.Base.Data.Seeder;
using MCMS.Base.Helpers;
using MCMS.Builder.Helpers;
using MCMS.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MCMS.Builder
{
    public class MApp : IMApp
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

            _specifications.ToList().ForEach(s => s.App = this);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (HostEnvironment.IsDevelopment())
            {
                services.AddDatabaseDeveloperPageExceptionFilter();
            }

            var mvcBuilder = services
                .AddMvc(options =>
                {
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
                    mvcJsonOptions.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                });

            mvcBuilder = _specifications.Aggregate(mvcBuilder,
                (current, mSpecifications) => mSpecifications.MvcChain(current));

            if (Env.GetBool("RAZOR_RUNTIME_COMPILATION"))
            {
                mvcBuilder.AddRazorRuntimeCompilation();
                RegisterPathsForRazorRuntimeCompilation(services);
            }

            new MAppEntitiesHelper(this).ProcessSpecifications(services);

            _addDbContextAction?.Invoke(services);

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


            services.AddOptions<FrameworkLibsDetails>().Configure(fld =>
            {
                foreach (var mSpec in _specifications)
                {
                    fld.Add(new FrameworkLibDetails(mSpec.GetType().Assembly));
                }

                var ioMcms = fld.Libs.FindIndex(l => l.Name == "MCMS");
                fld.Libs.Insert(ioMcms + 1, new FrameworkLibDetails(typeof(Forms.MCMSFormsHelper).Assembly));
            });
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UsePathBase(RoutePrefixes.RoutePrefix.TrimEnd('/'));

            app.UseForwardedHeaders();

            if (HostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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
                if (ctx.Request.Path.Value?.StartsWith("/api", StringComparison.OrdinalIgnoreCase) == true)
                {
                    var statusCodeFeature = ctx.Features.Get<IStatusCodePagesFeature>();

                    if (statusCodeFeature != null && statusCodeFeature.Enabled)
                        statusCodeFeature.Enabled = false;
                }

                await next();
            });

            MUseStaticFiles(app);

            app.UseRouting();

            InitializeDatabase(serviceProvider);

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

            // assert that variables are set correctly
            if (!(Env.GetOrThrow("EXTERNAL_URL") is { } url) || url.EndsWith('/') || !url.Contains("http"))
            {
                Utils.DieWith("EXTERNAL_URL must include protocol and must not end with /");
            }

            RoutePrefixes.CheckRoutePrefixVars();
        }


        private static void InitializeDatabase(IServiceProvider serviceProvider)
        {
            var migrateOnStart = Env.GetBool("MIGRATE_ON_START");
            var seedOnStart = Env.GetBool("SEED_ON_START");

            if (migrateOnStart || seedOnStart)
            {
                // create a new Scoped ServiceProvider in order to provide a DbContext (which is a scoped)
                serviceProvider = serviceProvider.CreateScope().ServiceProvider;
            }

            if (migrateOnStart)
            {
                EnsureDatabase(serviceProvider);
            }

            if (seedOnStart)
            {
                serviceProvider.GetRequiredService<DataSeeder>().SeedFromProvidedSources().Wait();
            }
        }

        private static void EnsureDatabase(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<MApp>>();
            logger.LogInformation("Checking pending migrations");
            var dbContext = serviceProvider.GetRequiredService<BaseDbContext>();
            var dbCreator = dbContext.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (dbCreator == null)
            {
                Utils.DieWith("Migrations: Couldn't get an instance of IDatabaseCreator.");
                return;
            }

            if (!dbCreator.Exists() && !dbContext.Database.GetMigrations().Any())
            {
                Utils.DieWith(
                    "There are no migrations in this project. Use 'dotnet ef migrations add initial' to create one.");
            }

            if (!dbCreator.Exists() || dbContext.Database.GetPendingMigrations().Any())
            {
                var migrationsStr = string.Join(", ", dbContext.Database.GetPendingMigrations());
                logger.LogWarning("Applying database migrations [{MigrationsStr}]", migrationsStr);
                dbContext.Database.Migrate();
            }
        }

        private void MUseStaticFiles(IApplicationBuilder app)
        {
            app.UseStaticFiles();

            // TODO: This is not used anymore. Should be deleted soon

            // var isPrePublish = Env.Get("LIB_STATIC_FILES_LOAD_TYPE") == "pre-publish";
            //
            // var specs = _specifications.Where(spec => spec.HasStaticFiles);
            //
            // foreach (var spec in specs)
            // {
            //     var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
            //         isPrePublish
            //             ? Path.Combine(spec.PrePublishRootPath ?? "", spec.GetAssemblyName(), "wwwroot")
            //             : Path.Combine("wwwroot/_content", spec.GetAssemblyName())
            //     ));
            //     app.UseStaticFiles(new StaticFileOptions
            //         {FileProvider = new PhysicalFileProvider(path), RequestPath = ""});
            // }
        }

        public void RegisterPathsForRazorRuntimeCompilation(IServiceCollection services)
        {
            var specs = _specifications.Where(spec => spec.HasRazorViews);
            foreach (var spec in specs)
            {
                var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), spec.PrePublishRootPath,
                    spec.GetAssemblyName()));
                services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
                {
                    options.FileProviders.Add(new PhysicalFileProvider(path));
                });
            }
        }
    }
}