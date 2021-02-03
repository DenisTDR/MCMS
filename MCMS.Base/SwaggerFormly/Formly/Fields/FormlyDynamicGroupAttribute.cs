using System;
using System.Collections.Generic;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyDynamicGroupAttribute : FormlySelectAttribute
    {
        public bool CanSelectExisting { get; set; } = true;
        public bool CanAddNewOrEdit { get; set; } = true;
        public FormlyEntityAddNewType AddNewType { get; set; }

        public FormlyDynamicGroupAttribute(string optionsUrl, string labelProp = "name", string valueProp = "id") :
            base(optionsUrl, labelProp, valueProp)
        {
            KeepAllOfSchemes = true;
            Type = "dynamic";
        }

        public FormlyDynamicGroupAttribute(Type optionsController, string actionName = "Index", string labelProp = "name",
            string valueProp = "id") : base(optionsController, actionName, labelProp, valueProp)
        {
            KeepAllOfSchemes = true;
            Type = "dynamic";
        }

        public override void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator,
            List<ValidatorModel> validators)
        {
            base.Patch(schema, xProps, templateOptions, linkGenerator, validators);
            templateOptions["requiredFromList"] = new OpenApiBoolean(true);
        }


        public override OpenApiObject GetCustomOpenApiConfig(LinkGenerator linkGenerator)
        {
            var baseConfig = base.GetCustomOpenApiConfig(linkGenerator);
            var types = new OpenApiArray();

            if (CanSelectExisting)
            {
                types.Add(new OpenApiObject
                {
                    {"type", new OpenApiString("autocomplete")}, {"label", new OpenApiString("Existing")},
                    {"validation", new OpenApiArray {new OpenApiString("required-from-list")}}
                });
                // types.Add(new OpenApiObject
                // {
                //     {"type", new OpenApiString("select")}, {"label", new OpenApiString("Existing S")},
                //     {"validation", new OpenApiArray {new OpenApiString("required-from-list")}}
                // });
            }

            if (CanAddNewOrEdit)
            {
                types.Add(new OpenApiObject
                {
                    {"type", new OpenApiString("subGroup")}, {"label", new OpenApiString("+ Add new / Edit")},
                    {"keepFieldGroup", new OpenApiBoolean(true)}
                });
            }

            baseConfig["fieldTypes"] = types;

            return baseConfig;
        }
    }

    public enum FormlyEntityAddNewType
    {
        Select,
        Autocomplete
    }
}