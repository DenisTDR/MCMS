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
        public bool OnlyIfUntouched { get; set; } = true;
        public bool CheckIfTouchedOnInit { get; set; } = true;
        public bool ForceEnableIfSourceChanged { get; set; } = true;

        public bool AlwaysEnabled { get; set; } = false;

        public string EnableExpression { get; set; }

        public override void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator, List<ValidatorModel> validators)
        {
            var config = new OpenApiObject
            {
                ["expression"] = new OpenApiString(Expression),
                ["enabled"] = new OpenApiBoolean(AlwaysEnabled || !OnlyIfUntouched),
                ["onlyIfUntouched"] = new OpenApiBoolean(OnlyIfUntouched && !AlwaysEnabled),
                ["checkIfTouchedOnInit"] = new OpenApiBoolean(CheckIfTouchedOnInit && !AlwaysEnabled),
                ["forceEnableIfSourceChanged"] = new OpenApiBoolean(ForceEnableIfSourceChanged && !AlwaysEnabled),
                ["enableExpression"] = new OpenApiString(EnableExpression),
            };
            templateOptions["autoFill"] = config;
        }
    }
}