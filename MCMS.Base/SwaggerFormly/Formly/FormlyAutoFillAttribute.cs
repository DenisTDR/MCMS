using System.Collections.Generic;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly
{
    public class FormlyAutoFillAttribute : FormlyConfigPatcherAttribute
    {
        public FormlyAutoFillAttribute(string expression)
        {
            Expression = expression;
        }

        public string Expression { get; set; }
        public bool OnlyIfUntouched { get; set; }
        public bool CheckIfTouchedOnInit { get; set; }
        public bool ForceEnableIfSourceChanged { get; set; }

        public override void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator, List<ValidatorModel> validators)
        {
            var config = new OpenApiObject
            {
                ["expression"] = new OpenApiString(Expression),
                ["enabled"] = new OpenApiBoolean(!OnlyIfUntouched),
                ["onlyIfUntouched"] = new OpenApiBoolean(OnlyIfUntouched),
                ["checkIfTouchedOnInit"] = new OpenApiBoolean(CheckIfTouchedOnInit),
                ["forceEnableIfSourceChanged"] = new OpenApiBoolean(ForceEnableIfSourceChanged)
            };
            templateOptions["autoFill"] = config;
        }
    }
}