namespace MCMS.Base.SwaggerFormly.Formly
{
    public class FormlyFieldDefaultValue : FormlyFieldPropAttribute
    {
        public FormlyFieldDefaultValue(object value) : base("defaultValue", value)
        {
        }
    }
}