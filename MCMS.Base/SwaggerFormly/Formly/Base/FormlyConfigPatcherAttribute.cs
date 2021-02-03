using System;
using System.Collections.Generic;
using MCMS.Base.SwaggerFormly.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MCMS.Base.SwaggerFormly.Formly.Base
{
    public class FormlyConfigPatcherAttribute : Attribute
    {
        public virtual void Patch(OpenApiSchema schema, OpenApiObject xProps, OpenApiObject templateOptions,
            LinkGenerator linkGenerator, List<ValidatorModel> validators)
        {
        }
    }
}