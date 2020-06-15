using System;

namespace MCMS.Base.SwaggerFormly
{
    public class CustomEnumValueAttribute : Attribute
    {
        public CustomEnumValueAttribute(object value)
        {
            Value = value;
        }

        public object Value { get; }
    }
}