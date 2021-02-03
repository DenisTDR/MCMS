using MCMS.Base.SwaggerFormly.Formly.Fields;

namespace MCMS.Base.SwaggerFormly.Formly
{
    public class FormlyIgnoreAttribute : FormlyFieldAttribute
    {
        public FormlyIgnoreAttribute()
        {
            IgnoreField = true;
        }
    }
}