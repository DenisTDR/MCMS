using System;

 namespace MCMS.Base.SwaggerFormly
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class BasicAttribute : Attribute
    {
        public string Name { get; }
        public object Value { get; }

        public BasicAttribute(string name, object value)
        {
            Value = value;
            Name = name;
        }
    }
}