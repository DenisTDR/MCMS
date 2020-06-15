using System;
using System.Linq;
using MCMS.Base.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using MCMS.SwaggerFormly.Extensions;

namespace MCMS.SwaggerFormly
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            // Patch only root level schemas, and not the members(properties) schemas of other schemas
            if (!context.Type.IsEnum || context.MemberInfo != null) return;

            model.Enum = Enum.GetValues(context.Type).Cast<Enum>()
                .Select(value => (IOpenApiAny) new OpenApiString(value.GetCustomValue().ToString()))
                .ToList();

            var templateOptions = model.Extensions.GetOrSetDefault("x-templateOptions", new OpenApiObject());
            var optionsArray = templateOptions.GetOrSetDefault("options", new OpenApiArray());
            optionsArray.AddRange(Enum.GetValues(context.Type).Cast<Enum>()
                .Select(enumValue =>
                {
                    var option = new OpenApiObject
                    {
                        ["value"] = OpenApiExtensions.ToOpenApi(enumValue.GetCustomValue()),
                        ["label"] = new OpenApiString(enumValue.GetDisplayName()),
                    };
                    if (enumValue.GetDisplayDescription() is { } s && !string.IsNullOrEmpty(s))
                    {
                        option["description"] = new OpenApiString(s);
                    }

                    return option;
                }));
        }
    }
}