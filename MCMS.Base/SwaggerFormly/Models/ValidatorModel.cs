using MCMS.Base.SwaggerFormly.Extensions;
using Microsoft.OpenApi.Any;

namespace MCMS.Base.SwaggerFormly.Models
{
    public class ValidatorModel
    {
        public ValidatorModel()
        {
        }

        public ValidatorModel(string name, object args = null, string message = null)
        {
            Name = name;
            Message = message;
            Args = args;
        }

        public string Name { get; set; }
        public string Message { get; set; }
        public object Args { get; set; }

        public OpenApiObject ToOpenApiObject()
        {
            var oaObject = new OpenApiObject
            {
                {"name", new OpenApiString(Name)}
            };
            if (Args?.ToString() != null)
            {
                oaObject["args"] = OpenApiExtensions.ToOpenApi(Args);
            }

            if (!string.IsNullOrEmpty(Message))
            {
                oaObject["message"] = new OpenApiString(Message);
            }

            return oaObject;
        }
    }
}