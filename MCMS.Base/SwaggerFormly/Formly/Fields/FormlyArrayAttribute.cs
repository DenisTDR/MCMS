using System.Collections.Generic;
using MCMS.Base.SwaggerFormly.Extensions;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyArrayAttribute : FormlyCustomFieldFieldAttribute
    {
        public bool RemoveDisabled { get; set; }
        public bool AddDisabled { get; set; }
        public string FieldGroupClassName { get; set; }
        public string AddButtonContent { get; set; }
        public string RemoveButtonContent { get; set; }
        public bool Sortable { get; set; }

        public override void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator,
            List<ValidatorModel> validators)
        {
            base.Patch(schema, xProps, templateOptions, linkGenerator, validators);
            if (FieldGroupClassName != null)
            {
                xProps["fieldGroupClassName"] = OpenApiExtensions.ToOpenApi(FieldGroupClassName);
            }
        }

        public override OpenApiObject GetCustomOpenApiConfig(LinkGenerator linkGenerator)
        {
            var obj = new OpenApiObject();
            if (RemoveDisabled)
            {
                obj["removeDisabled"] = new OpenApiBoolean(true);
            }

            if (AddDisabled)
            {
                obj["addDisabled"] = new OpenApiBoolean(true);
            }

            if (!string.IsNullOrEmpty(AddButtonContent))
            {
                obj["addButtonContent"] = new OpenApiString(AddButtonContent);
            }

            if (!string.IsNullOrEmpty(RemoveButtonContent))
            {
                obj["removeButtonContent"] = new OpenApiString(RemoveButtonContent);
            }

            return obj;
        }
    }
}