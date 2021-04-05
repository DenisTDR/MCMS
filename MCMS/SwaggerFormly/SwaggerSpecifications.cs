using System;
using System.Linq;
using MCMS.Base.Builder;
using MCMS.Base.Files.UploadPurpose;
using MCMS.Base.Helpers;
using MCMS.Base.Middlewares;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MCMS.SwaggerFormly.Filters;
using MCMS.SwaggerFormly.FormParamsHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.ReDoc;

namespace MCMS.SwaggerFormly
{
    public class SwaggerSpecifications : MSpecifications
    {
        private readonly SwaggerConfigsOptions _configsOptions;

        public SwaggerSpecifications(params SwaggerConfigOptions[] configOptions)
        {
            _configsOptions = new SwaggerConfigsOptions
            {
                ForAdmin = configOptions.Length > 0 ? configOptions[0] : null,
                ForApi = configOptions.Length > 1 ? configOptions[1] : null,
            };
            if (configOptions.Length > 2)
            {
                _configsOptions.CustomConfigs.AddRange(configOptions.Skip(2));
            }

            _configsOptions.PatchMainConfigs();
        }

        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            if (Env.GetBool("FORMLY_DEBUG"))
            {
                var logger = serviceProvider.GetRequiredService<ILogger<SwaggerSpecifications>>();
                logger.LogWarning("FORMLY_DEBUG=True => enabling reverse proxy middleware");
                var obj = new ReverseProxyMiddlewareOptions
                {
                    ProxyRules = new()
                    {
                        {"/mcms-forms", Utils.UrlCombine(Env.GetOrThrow("FORMLY_SERVE_URL"), "mcms-forms/")}
                    }
                };
                app.UseMiddleware<ReverseProxyMiddleware>(obj);
            }

            app.UseSwagger(options => options.RouteTemplate = _configsOptions.ForAdmin.RouteTemplate);

            RegisterUi(app, serviceProvider.GetRequiredService<IOptions<SwaggerConfigsOptions>>().Value);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<SwaggerConfigsOptions>().Configure(c => { _configsOptions.SetupSwallowClone(c); });
            services.AddSwaggerGen(swagger =>
            {
                foreach (var swaggerConfigOptions in _configsOptions.GetAll())
                {
                    swagger.SwaggerDoc(swaggerConfigOptions.Name, swaggerConfigOptions.ToOpenApiInfo());
                }

                swagger.SchemaFilter<OpenApiFormlyPatcherSchemaFilter>();
                swagger.OperationFilter<DefaultSummaryOperationFilter>();
                swagger.SchemaFilter<EnumSchemaFilter>();
                swagger.UseAllOfToExtendReferenceSchemas();
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddScoped(typeof(FormParamsForControllerService<,>));
            services.AddScoped<FormParamsServiceBuilder>();
            services.AddSingleton<SwaggerConfigService>();
            services.AddOptions<UploadPurposeOptions>().Configure(o =>
            {
                var ckeditorPurpose = new FileUploadPurpose("ckeditor")
                {
                    Accept = new[] {"jpg", "jpeg", "png", "gif", "bmp", "webp", "tiff", "svg"}
                };
                o.Register("ckeditor", ckeditorPurpose);
            });
        }


        private void RegisterUi(IApplicationBuilder app, SwaggerConfigsOptions configs)
        {
            var prefix = RoutePrefixes.RoutePrefix;

            var allConfigs = configs.GetAll().ToList();
            if (allConfigs.Any(c => c.UseSwaggerUi()))
            {
                app.UseSwaggerUI(uiOptions =>
                {
                    foreach (var swaggerConfigOptions in allConfigs)
                    {
                        uiOptions.SwaggerEndpoint(swaggerConfigOptions.GetEndpointUrl(prefix),
                            swaggerConfigOptions.GetEndpointName());
                    }

                    foreach (var javascriptFile in configs.JavascriptFiles)
                    {
                        uiOptions.InjectJavascript(Utils.UrlCombine(prefix, javascriptFile));
                    }

                    foreach (var stylesheetFile in configs.StylesheetFiles)
                    {
                        uiOptions.InjectStylesheet(Utils.UrlCombine(prefix, stylesheetFile));
                    }

                    uiOptions.RoutePrefix = "api/docs";
                });
            }

            var redocSwaggerSpecs = allConfigs.Where(swaggerConfigOptions => swaggerConfigOptions.UseReDoc()).ToList();
            foreach (var redocSP in redocSwaggerSpecs)
            {
                var redocOptions = new ReDocOptions
                {
                    RoutePrefix = "api/redoc/" + redocSP.Name,
                    SpecUrl = redocSP.GetEndpointUrl(),
                    DocumentTitle = redocSP.Title + " | ReDoc"
                };

                if (redocSwaggerSpecs.Count > 1)
                {
                    var switchDocConfigObj = new
                    {
                        current = redocSP.Name,
                        docs = redocSwaggerSpecs.Select(rss => new {rss.Name, rss.Title})
                    };
                    redocOptions.HeadContent =
                        $"<script>\nvar switchDocConfig = {Utils.Serialize(switchDocConfigObj)};\n</script>\n"
                        + "<script src='/_content/MCMS/api/redoc/redoc-doc-switcher.js'></script>";
                }


                app.UseReDoc(redocOptions);
            }
        }
    }
}