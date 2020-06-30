using System;
using Microsoft.OpenApi.Any;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FormlyFieldAttribute : Attribute
    {
        public FormlyFieldAttribute()
        {
            
        }
        public FormlyFieldAttribute(string type)
        {
            Type = type;
        }

        public string Type { get; set; }
        
        public virtual OpenApiObject GetOpenApiConfig()
        {
           return null;
        }
    }
}