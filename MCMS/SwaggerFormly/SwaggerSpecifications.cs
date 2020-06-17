using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MCMS.Builder;
using MCMS.Helpers;
using MCMS.SwaggerFormly.Filters;
using MCMS.SwaggerFormly.FormParamsHelpers;
using MCMS.SwaggerFormly.Middlewares;
using MCMS.SwaggerFormly.Models;

namespace MCMS.SwaggerFormly
{
    public class SwaggerSpecifications : MSpecifications
    {
        private readonly SwaggerConfigOptions _swaggerConfigOptions;

        public SwaggerSpecifications(SwaggerConfigOptions swaggerConfigOptions)
        {
            _swaggerConfigOptions = swaggerConfigOptions;
        }

        public override void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            if (Env.GetBool("FORMLY_DEBUG"))
            {
                app.UseMiddleware<ReverseProxyMiddleware>();
            }

            app.UseSwagger(options => options.RouteTemplate = _swaggerConfigOptions.RouteTemplate);
            app.UseSwaggerUI(uiOptions =>
            {
                uiOptions.SwaggerEndpoint($"/api/docs/{_swaggerConfigOptions.Name}/swagger.json",
                    _swaggerConfigOptions.Title + " " + _swaggerConfigOptions.Version);
                uiOptions.InjectStylesheet("/api/docs/swagger-ui-theme.css");
                uiOptions.InjectJavascript("/api/docs/swagger-ui-theme.js");
                uiOptions.RoutePrefix = "api/docs";
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<SwaggerConfigOptions>().Configure(c => { c.Name = _swaggerConfigOptions.Name; });
            services.AddSwaggerGen(swagger =>
            {
                _swaggerConfigOptions.Description =
                    "<a href='/'>Back to home page</a>" + _swaggerConfigOptions.Description;
                swagger.SwaggerDoc(_swaggerConfigOptions.Name,
                    new OpenApiInfo
                    {
                        Title = _swaggerConfigOptions.Title,
                        Version = _swaggerConfigOptions.Version,
                        Description = _swaggerConfigOptions.Description,
                    });
                swagger.SchemaFilter<OpenApiFormlyPatcherSchemaFilter>();
                swagger.SchemaFilter<EnumSchemaFilter>();
                swagger.UseAllOfToExtendReferenceSchemas();
            });

            services.AddTransient(typeof(FormParamsForControllerService<,>));
            services.AddTransient<FormParamsServiceBuilder>();
        }
    }
}