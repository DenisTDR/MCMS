using System.Reflection;
using MCMS.Base.Helpers;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyTextAttribute : FormlyCustomFieldFieldAttribute
    {
        public FormlyTextAttribute() : base("text")
        {
        }

        public override string GetDisplayName(PropertyInfo prop)
        {
            return TypeHelpers.GetDisplayName(prop);
        }
    }
}