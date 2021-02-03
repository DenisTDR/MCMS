using System;
using System.Collections.Generic;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Base.SwaggerFormly.Formly.Base;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormlyEntityFillAttribute : FormlyConfigPatcherAttribute
    {
        public string OptionsUrl { get; }
        public Type OptionsControllerType { get; }
        public string OptionsActionName { get; }
        public string LabelProp { get; set; } = "name";
        public string ValueProp { get; set; } = "id";

        public string SelectorFieldType { get; set; }
        public string KeepId { get; set; }

        public FormlyEntityFillAttribute(string optionsUrl)
        {
            OptionsUrl = optionsUrl;
        }

        public FormlyEntityFillAttribute(Type optionsController, string actionName = "Index")
        {
            OptionsActionName = actionName;
            OptionsControllerType = optionsController;
        }

        public override void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator,
            List<ValidatorModel> validators)
        {
            base.Patch(schema, xProps, templateOptions, linkGenerator, validators);

            var customConfig = templateOptions.GetOrSetDefault<OpenApiObject, IOpenApiAny>("customFieldConfig");

            customConfig["labelProp"] = new OpenApiString(LabelProp);
            customConfig["valueProp"] = new OpenApiString(ValueProp);

            var optionsUrl = OptionsControllerType != null
                ? linkGenerator.GetAbsolutePathByAction(OptionsActionName,
                    TypeHelpers.GetControllerName(OptionsControllerType))
                : OptionsUrl;
            customConfig["loadOptionsFromUrl"] = new OpenApiBoolean(true);
            customConfig["optionsUrl"] = new OpenApiString(optionsUrl);

            var fieldGroupFill = customConfig.GetOrSetDefault<OpenApiObject, IOpenApiAny>("fieldGroupFill");

            fieldGroupFill["enabled"] = new OpenApiBoolean(true);
            fieldGroupFill["keepId"] = new OpenApiBoolean(true);
            fieldGroupFill["selectorFieldType"] = new OpenApiString(SelectorFieldType ?? "autocomplete");
        }
    }
}