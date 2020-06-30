namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyAutocompleteAttribute : FormlySelectAttribute
    {
        public FormlyAutocompleteAttribute(string optionsUrl, string labelProp = "name", string valueProp = "id") : base(
            optionsUrl, labelProp, valueProp)
        {
        }
    }
}