using System;

namespace MCMS.Base.SwaggerFormly.Formly
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormlyIgnoreAttribute : Attribute
    {
        public bool DontIgnore { get; set; }
    }
}