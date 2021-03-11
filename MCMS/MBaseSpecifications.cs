using System;
using System.IO;
using MCMS.Base.Builder;
using MCMS.Base.Data;
using MCMS.Base.Data.Seeder;
using MCMS.Base.Display.DisplayValue;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MCMS.Data;
using MCMS.Display.DetailsConfig;
using MCMS.Display.TableConfig;
using MCMS.Filters;
using MCMS.Forms;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Z.Expressions;

namespace MCMS
{
    public class MBaseSpecifications : MSpecifications
    {
        public MBaseSpecifications()
        {
            HasRazorViews = true;
            HasStaticFiles = true;
            PrePublishRootPath = "../MCMS";
        }

        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            AddCorsFromEnv(app);
            app.UseMCMSFormsStaticFiles();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<DataSeeder>();

            EvalManager.DefaultContext.RegisterType(typeof(MDbFunctions));
            services.AddScoped(typeof(IDetailsConfigServiceT<>), typeof(DetailsConfigService<>));
            services.AddScoped(typeof(ITableConfigServiceT<>), typeof(TableConfigService<>));
            services.AddScoped(typeof(TableConfigForControllerService<,,,,>));

            services.AddSingleton<DisplayValueService>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient(serviceProvider => serviceProvider
                .GetRequiredService<IUrlHelperFactory>()
                .GetUrlHelper(serviceProvider.GetRequiredService<IActionContextAccessor>().ActionContext));

            services.AddTransient<IAdapterFactory, AdapterFactory>();

            services.AddOptions<DisplayValueFormatters>().Configure(DefaultDisplayValueFormatters.RegisterFormatters);

            services.AddScoped(typeof(DtQueryService<>));

            if (!string.IsNullOrEmpty(Env.Get("PERSISTED_KEYS_DIRECTORY")))
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(Env.Get("PERSISTED_KEYS_DIRECTORY")));
            }
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