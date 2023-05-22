using System;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyCheckListAttribute : FormlyCustomFieldFieldAttribute
    {
        public string OptionsUrl { get; }
        public Type OptionsControllerType { get; }
        public string OptionsActionName { get; }
        public string LabelProp { get; }
        public string ValueProp { get; }
        public string OptionClass { get; set; }


        public FormlyCheckListAttribute(Type optionsController, string actionName = "Index", string labelProp = "label",
            string valueProp = "value")
        {
            OptionsActionName = actionName;
            OptionsControllerType = optionsController;
            LabelProp = labelProp;
            ValueProp = valueProp;
            Type = "checklist";
        }

        public FormlyCheckListAttribute(string optionsUrl, string labelProp = "name", string valueProp = "id")
        {
            OptionsUrl = optionsUrl;
            LabelProp = labelProp;
            ValueProp = valueProp;
            Type = "checklist";
        }


        public override OpenApiObject GetCustomOpenApiConfig(LinkGenerator linkGenerator)
        {
            var obj = new OpenApiObject
            {
                ["labelProp"] = new OpenApiString(LabelProp),
                ["valueProp"] = new OpenApiString(ValueProp)
            };
            var optionsUrl = OptionsControllerType != null
                ? linkGenerator.GetAbsolutePathByAction(OptionsActionName,
                    TypeHelpers.GetControllerName(OptionsControllerType))
                : OptionsUrl;
            obj["loadOptionsFromUrl"] = new OpenApiBoolean(true);
            obj["optionsUrl"] = new OpenApiString(optionsUrl);
            obj["optionClass"] = new OpenApiString(OptionClass);
            return obj;
        }
    }
}