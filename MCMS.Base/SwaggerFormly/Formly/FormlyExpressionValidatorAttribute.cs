using System.Collections.Generic;
using MCMS.Base.SwaggerFormly.Formly.Base;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly
{
    public class FormlyExpressionValidatorAttribute : FormlyConfigPatcherAttribute
    {
        public string Expression { get; set; }
        public string Message { get; set; }
        public string Key { get; set; }

        public FormlyExpressionValidatorAttribute(string expression)
        {
            Expression = expression;
        }


        public override void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator, List<ValidatorModel> validators)
        {
            validators.Add(new ValidatorModel
            {
                Name = "expressionValidator",
                Args = new OpenApiObject
                {
                    {"expression", new OpenApiString(Expression)},
                    {"key", new OpenApiString(Key ?? "expressionValidatorInvalid")}
                },
                Message = Message
            });
        }
    }
}