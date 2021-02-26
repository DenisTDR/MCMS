using System.Collections.Generic;
using System.Reflection;
using MCMS.Base.Helpers;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyButtonAttribute : FormlyCustomFieldFieldAttribute
    {
        public string ButtonClasses { get; set; }
        public string ActionExpression { get; set; }
        public string ActionTarget { get; set; }
        public bool FakeLabel { get; set; }

        public FormlyButtonAttribute(string actionTarget, string actionExpression = null, string buttonClasses = null) :
            base("button")
        {
            ActionExpression = actionExpression;
            ButtonClasses = buttonClasses;
            ActionTarget = actionTarget;
        }

        public override string GetDisplayName(PropertyInfo prop)
        {
            return TypeHelpers.GetDisplayName(prop);
        }

        public override void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator,
            List<ValidatorModel> validators)
        {
            base.Patch(schema, xProps, templateOptions, linkGenerator, validators);
            templateOptions["buttonClasses"] = new OpenApiString(ButtonClasses);
            templateOptions["actionExpression"] = new OpenApiString(ActionExpression);
            templateOptions["actionTarget"] = new OpenApiString(ActionTarget);
            templateOptions["fakeLabel"] = new OpenApiBoolean(FakeLabel);
        }
    }
}