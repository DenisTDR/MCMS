namespace MCMS.Base.SwaggerFormly.Formly
{
    public class FormlyFieldDefaultValueAttribute : FormlyFieldPropAttribute
    {
        public FormlyFieldDefaultValueAttribute(object value) : base("defaultValue", value)
        {
        }
    }
}