using System.Collections.Generic;
using MCMS.Base.SwaggerFormly.Extensions;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyCustomFieldFieldAttribute : FormlyFieldAttribute
    {
        public FormlyCustomFieldFieldAttribute()
        {
        }

        public FormlyCustomFieldFieldAttribute(string type, string format = null)
        {
            Type = type;
            Format = format;
        }

        public bool AsOpenApi { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }


        public virtual OpenApiObject GetOpenApiConfig(LinkGenerator linkGenerator)
        {
            return null;
        }

        public override void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator, List<ValidatorModel> validators)
        {
            base.Patch(schema, xProps, templateOptions, linkGenerator, validators);
            schema.AllOf = new List<OpenApiSchema>();
            if (!string.IsNullOrEmpty(Type))
            {
                if (AsOpenApi)
                {
                    schema.Type = Type;
                }
                else
                {
                    xProps["type"] = OpenApiExtensions.ToOpenApi(Type);
                }
            }

            var customFieldConfig = GetOpenApiConfig(linkGenerator);
            if (customFieldConfig != null)
            {
                templateOptions["customFieldConfig"] = customFieldConfig;
            }

            if (Format != null)
            {
                if (AsOpenApi)
                {
                    schema.Format = Format;
                }
                else
                {
                    templateOptions["format"] = OpenApiExtensions.ToOpenApi(Format);
                }
            }
        }
    }
}