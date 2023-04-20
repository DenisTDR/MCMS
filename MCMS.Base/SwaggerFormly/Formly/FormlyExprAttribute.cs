using MCMS.Base.SwaggerFormly.Formly.Base;

namespace MCMS.Base.SwaggerFormly.Formly
{
    public class FormlyExprAttribute : FormlyFieldPropAttribute 
    { 
        public FormlyExprAttribute(string name, object value) 
            : base(name, value, "expressionProperties") 
        { 
        } 
    } 
}