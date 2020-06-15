using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MCMS.Base.SwaggerFormly
{
    public class RequireNotDefaultAttribute : RequiredAttribute
    {
        private readonly Type _propertyType;

        public RequireNotDefaultAttribute([NotNull] Type propertyType)
        {
            if (propertyType == null)
            {
                throw new ArgumentException("The provided type is null.");
            }

            if (!propertyType.IsValueType)
            {
                throw new ArgumentException($"The provided type '{_propertyType.Name}' is not a value type.");
            }

            _propertyType = propertyType;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return false;
            return value != Activator.CreateInstance(_propertyType);
        }
    }
}