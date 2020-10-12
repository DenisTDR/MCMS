using System.ComponentModel.DataAnnotations;

namespace MCMS.Base.SwaggerFormly
{
    public class FormOnlyRequiredAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return true;
        }
    }
}