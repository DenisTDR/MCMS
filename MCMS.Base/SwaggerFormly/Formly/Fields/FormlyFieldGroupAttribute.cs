using System.Collections.Generic;
using MCMS.Base.SwaggerFormly.Extensions;
using MCMS.Base.SwaggerFormly.Formly.Base;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyFieldGroupAttribute : FormlyConfigPatcherAttribute
    {
        public string FieldGroupClassName { get; set; }

        public override void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator, List<ValidatorModel> validators)
        {
            base.Patch(schema, xProps, templateOptions, linkGenerator, validators);
            if (!string.IsNullOrEmpty(FieldGroupClassName))
            {
                xProps["fieldGroupClassName"] = OpenApiExtensions.ToOpenApi(FieldGroupClassName);
            }
        }
    }
}