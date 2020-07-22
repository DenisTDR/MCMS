using System;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyAutocompleteAttribute : FormlySelectAttribute
    {
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
    }
}