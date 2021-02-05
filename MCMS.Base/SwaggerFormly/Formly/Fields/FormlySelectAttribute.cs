using System;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlySelectAttribute : FormlyCustomFieldFieldAttribute
    {
        public string OptionsUrl { get; }
        public Type OptionsControllerType { get; }
        public string OptionsActionName { get; }
        public string LabelProp { get; }
        public string ValueProp { get; }
        
        public bool ReloadOptionsOnInit { get; set; }
        public bool ShowReloadButton { get; set; }


        public FormlySelectAttribute(string optionsUrl, string labelProp = "name", string valueProp = "id")
        {
            OptionsUrl = optionsUrl;
            LabelProp = labelProp;
            ValueProp = valueProp;
            Type = "select";
        }

        public FormlySelectAttribute(Type optionsController, string actionName = "Index", string labelProp = "name",
            string valueProp = "id")
        {
            OptionsActionName = actionName;
            OptionsControllerType = optionsController;
            LabelProp = labelProp;
            ValueProp = valueProp;
            Type = "select";
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
            obj["reloadOptionsOnInit"] = new OpenApiBoolean(ReloadOptionsOnInit);
            obj["showReloadButton"] = new OpenApiBoolean(ShowReloadButton);
            return obj;
        }
    }
}