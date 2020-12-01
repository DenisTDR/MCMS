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
        private readonly SwaggerConfigOptions _swaggerConfigOptions;
        private readonly SwaggerConfigOptions _apiSwaggerConfigOptions;

        public SwaggerSpecifications(SwaggerConfigOptions swaggerConfigOptions,
            SwaggerConfigOptions apiSwaggerConfigOptions)
        {
            _swaggerConfigOptions = swaggerConfigOptions;
            _apiSwaggerConfigOptions = apiSwaggerConfigOptions;
        }

        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            if (Env.GetBool("FORMLY_DEBUG"))
            {
                var logger = serviceProvider.GetRequiredService<ILogger<SwaggerSpecifications>>();
                logger.LogWarning("FORMLY_DEBUG=True => enabling reverse proxy middleware");
                app.UseMiddleware<ReverseProxyMiddleware>();
            }

            app.UseSwagger(options => options.RouteTemplate = _swaggerConfigOptions.RouteTemplate);
            var prefix = RoutePrefixes.RoutePrefix;
            app.UseSwaggerUI(uiOptions =>
            {
                uiOptions.SwaggerEndpoint(
                    Utils.UrlCombine(prefix, $"api/docs/{_swaggerConfigOptions.Name}/swagger.json"),
                    _swaggerConfigOptions.Title + " " + _swaggerConfigOptions.Version);
                if (_apiSwaggerConfigOptions != null)
                {
                    uiOptions.SwaggerEndpoint(
                        Utils.UrlCombine(prefix, $"api/docs/{_apiSwaggerConfigOptions.Name}/swagger.json"),
                        _apiSwaggerConfigOptions.Title + " " + _apiSwaggerConfigOptions.Version);
                }

                uiOptions.InjectStylesheet(Utils.UrlCombine(prefix, "api/docs/swagger-ui-theme.css"));
                uiOptions.InjectJavascript(Utils.UrlCombine(prefix, "api/docs/swagger-ui-theme.js"));
                uiOptions.RoutePrefix = "api/docs";
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            _swaggerConfigOptions.Name = "admin-api";
            if (_apiSwaggerConfigOptions != null)
            {
                _apiSwaggerConfigOptions.Name = "api";
            }

            services.AddOptions<SwaggerConfigOptions>().Configure(c => { c.Name = _swaggerConfigOptions.Name; });
            services.AddSwaggerGen(swagger =>
            {
                PatchDescriptions();
                swagger.SwaggerDoc("admin-api",
                    new OpenApiInfo
                    {
                        Title = _swaggerConfigOptions.Title,
                        Version = _swaggerConfigOptions.Version,
                        Description = _swaggerConfigOptions.Description,
                    });
                if (_apiSwaggerConfigOptions != null)
                {
                    swagger.SwaggerDoc(_apiSwaggerConfigOptions.Name,
                        new OpenApiInfo
                        {
                            Title = _apiSwaggerConfigOptions.Title,
                            Version = _apiSwaggerConfigOptions.Version,
                            Description = _apiSwaggerConfigOptions.Description,
                        });
                }

                swagger.SchemaFilter<OpenApiFormlyPatcherSchemaFilter>();
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

        private void PatchDescriptions()
        {
            var homeUrl = $"<a href='{RoutePrefixes.RoutePrefix}'>Back to home page</a>";
            _swaggerConfigOptions.Description = $"{homeUrl}{_swaggerConfigOptions.Description}";
            if (_apiSwaggerConfigOptions != null)
            {
                _apiSwaggerConfigOptions.Description = $"{homeUrl}{_apiSwaggerConfigOptions.Description}";
            }
        }
    }
}