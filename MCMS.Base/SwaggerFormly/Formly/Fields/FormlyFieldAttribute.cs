using System;
using System.Collections.Generic;
using MCMS.Base.SwaggerFormly.Extensions;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormlyFieldAttribute : Attribute
    {
        public FormlyFieldAttribute()
        {
        }

        public FormlyFieldAttribute(string type, string format = null)
        {
            Type = type;
            Format = format;
        }

        public bool AsOpenApi { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        
        public bool HasCustomValidators { get; set; }

        public virtual OpenApiObject GetOpenApiConfig(LinkGenerator linkGenerator)
        {
            return null;
        }

        public virtual void Attach(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator)
        {
            schema.AllOf = new List<OpenApiSchema>();
            if (AsOpenApi)
            {
                schema.Type = Type;
            }
            else
            {
                xProps["type"] = OpenApiExtensions.ToOpenApi(Type);
            }

            var typeConfig = GetOpenApiConfig(linkGenerator);
            if (typeConfig != null)
            {
                templateOptions["type-config"] = typeConfig;
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

        public virtual List<ValidatorModel> GetCustomValidators()
        {
            return null;
        }
    }
}