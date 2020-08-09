using System;
using System.Collections.Generic;
using System.Linq;
using MCMS.Base.SwaggerFormly.Extensions;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FormlyFieldAttribute : Attribute
    {
        public bool IgnoreField { get; set; }
        public double OrderIndex { get; set; }
        public object DefaultValue { get; set; }
        public string[] Wrappers { get; set; }
        public string ClassName { get; set; }
        public bool HasCustomValidators { get; set; }


        public virtual void Attach(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator)
        {
            AttachBasicProps(schema, xProps, templateOptions, linkGenerator);
        }

        protected virtual void AttachBasicProps(OpenApiSchema schema, OpenApiObject xProps,
            OpenApiObject templateOptions,
            LinkGenerator linkGenerator)
        {
            if (ClassName != null)
            {
                xProps["className"] = OpenApiExtensions.ToOpenApi(ClassName);
            }

            if (DefaultValue != null)
            {
                xProps["defaultValue"] = OpenApiExtensions.ToOpenApi(DefaultValue);
            }

            if (Wrappers != null)
            {
                var arr = new OpenApiArray();
                arr.AddRange(from object o in Wrappers select OpenApiExtensions.ToOpenApi(o));
                xProps["wrappers"] = arr;
            }
        }

        public virtual List<ValidatorModel> GetCustomValidators()
        {
            return null;
        }
    }
}