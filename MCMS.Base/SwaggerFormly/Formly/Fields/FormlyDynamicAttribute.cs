using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Oae = MCMS.Base.SwaggerFormly.Extensions.OpenApiExtensions;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyDynamicAttribute : FormlyCustomFieldFieldAttribute
    {
        public List<(string, string)> Types { get; set; }

        public FormlyDynamicAttribute(params string[] ps)
        {
            Types = new List<(string, string)>();
            for (var i = 0; i < ps.Length; i += 2)
            {
                Types.Add((ps[i], ps[i + 1]));
            }

            Type = "dynamic";
        }

        public override OpenApiObject GetCustomOpenApiConfig(LinkGenerator linkGenerator)
        {
            var baseConfig = base.GetCustomOpenApiConfig(linkGenerator) ?? new OpenApiObject();
            var types = new OpenApiArray();

            foreach (var valueTuple in Types)
            {
                types.Add(new OpenApiObject
                {
                    {"type", Oae.ToOpenApi(valueTuple.Item1)},
                    {"label", Oae.ToOpenApi(valueTuple.Item2)},
                });
            }

            baseConfig["fieldTypes"] = types;

            return baseConfig;
        }
    }
}