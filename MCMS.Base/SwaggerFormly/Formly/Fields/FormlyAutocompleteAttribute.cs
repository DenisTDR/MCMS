using System;
using System.Collections.Generic;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyAutocompleteAttribute : FormlySelectAttribute
    {
        public bool CanAddNew { get; set; }

        public FormlyAutocompleteAttribute(string optionsUrl, string labelProp = "name", string valueProp = "id") :
            base(optionsUrl, labelProp, valueProp)
        {
            Type = "autocomplete";
        }

        public FormlyAutocompleteAttribute(Type optionsController, string actionName = "Index",
            string labelProp = "name", string valueProp = "id") : base(optionsController, actionName, labelProp,
            valueProp)
        {
            Type = "autocomplete";
        }

        public override void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator, List<ValidatorModel> validators)
        {
            base.Patch(schema, xProps, templateOptions, linkGenerator, validators);
            if (!CanAddNew)
            {
                validators.Add(new ValidatorModel("required-from-list"));
                templateOptions["requiredFromList"] = new OpenApiBoolean(true);
            }
        }
    }
}