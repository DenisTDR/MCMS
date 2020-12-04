using System;
using MCMS.Base.Builder;
using MCMS.Base.Files.UploadPurpose;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MCMS.SwaggerFormly.Filters;
using MCMS.SwaggerFormly.FormParamsHelpers;
using MCMS.SwaggerFormly.Middlewares;
using MCMS.SwaggerFormly.Models;
using Microsoft.Extensions.Logging;

namespace MCMS.SwaggerFormly
{
    public class SwaggerSpecifications : MSpecifications
    {
        private readonly SwaggerConfigOptions _adminConfigOptions;
        private readonly SwaggerConfigOptions _apiConfigOptions;

        public SwaggerSpecifications(SwaggerConfigOptions adminConfigOptions, SwaggerConfigOptions apiConfigOptions)
        {
            _adminConfigOptions = adminConfigOptions;
            _apiConfigOptions = apiConfigOptions;
        }

        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            if (Env.GetBool("FORMLY_DEBUG"))
            {
                var logger = serviceProvider.GetRequiredService<ILogger<SwaggerSpecifications>>();
                logger.LogWarning("FORMLY_DEBUG=True => enabling reverse proxy middleware");
                app.UseMiddleware<ReverseProxyMiddleware>();
            }

            app.UseSwagger(options => options.RouteTemplate = _adminConfigOptions.RouteTemplate);

            RegisterUi(app);
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            _adminConfigOptions.Name = "admin-api";
            if (_apiConfigOptions != null)
            {
                _apiConfigOptions.Name = "api";
            }

            services.AddOptions<SwaggerConfigOptions>().Configure(c => { c.Name = _adminConfigOptions.Name; });
            services.AddSwaggerGen(swagger =>
            {
                PatchDescriptions();
                swagger.SwaggerDoc(_adminConfigOptions.Name,
                    new OpenApiInfo
                    {
                        Title = _adminConfigOptions.Title,
                        Version = _adminConfigOptions.Version,
                        Description = _adminConfigOptions.Description,
                    });
                if (_apiConfigOptions != null)
                {
                    swagger.SwaggerDoc(_apiConfigOptions.Name,
                        new OpenApiInfo
                        {
                            Title = _apiConfigOptions.Title,
                            Version = _apiConfigOptions.Version,
                            Description = _apiConfigOptions.Description,
                        });
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


        private void RegisterUi(IApplicationBuilder app)
        {
            var prefix = RoutePrefixes.RoutePrefix;

            if (_adminConfigOptions.UiType.UseSwaggerUi() || _apiConfigOptions?.UiType.UseSwaggerUi() == true)
            {
                app.UseSwaggerUI(uiOptions =>
                {
                    if (_adminConfigOptions.UiType.UseSwaggerUi())
                    {
                        uiOptions.SwaggerEndpoint(_adminConfigOptions.GetEndpointUrl(prefix),
                            _adminConfigOptions.GetEndpointName());
                    }

                    if (_apiConfigOptions?.UiType.UseSwaggerUi() == true)
                    {
                        uiOptions.SwaggerEndpoint(_apiConfigOptions.GetEndpointUrl(prefix),
                            _apiConfigOptions.GetEndpointName());
                    }

                    uiOptions.InjectStylesheet(Utils.UrlCombine(prefix, "api/docs/swagger-ui-theme.css"));
                    uiOptions.InjectJavascript(Utils.UrlCombine(prefix, "api/docs/swagger-ui-theme.js"));
                    uiOptions.RoutePrefix = "api/docs";
                });
            }

            if (_adminConfigOptions.UiType.UseReDoc())
            {
                app.UseReDoc(c =>
                {
                    c.RoutePrefix = "api/redoc/" + _adminConfigOptions.Name;
                    c.SpecUrl(_adminConfigOptions.GetEndpointUrl());
                });
            }

            if (_apiConfigOptions?.UiType.UseReDoc() == true)
            {
                app.UseReDoc(c =>
                {
                    c.RoutePrefix = "api/redoc/" + _apiConfigOptions.Name;
                    c.SpecUrl(_apiConfigOptions.GetEndpointUrl());
                });
            }
        }

        private void PatchDescriptions()
        {
            var homeUrl =
                $"<a href='{Utils.UrlCombine(RoutePrefixes.RoutePrefix, RoutePrefixes.AdminRoutePrefix.Substring(1))}'>Back to admin page</a>";
            _adminConfigOptions.Description = $"{homeUrl}{_adminConfigOptions.Description}";
            if (_apiConfigOptions != null)
            {
                _apiConfigOptions.Description = $"{homeUrl}{_apiConfigOptions.Description}";
            }
        }
    }
}