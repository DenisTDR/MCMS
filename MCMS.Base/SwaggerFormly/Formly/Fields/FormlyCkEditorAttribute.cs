using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;

namespace MCMS.Base.SwaggerFormly.Formly.Fields
{
    public class FormlyCkEditorAttribute : FormlyCustomFieldFieldAttribute
    {
        public FormlyCkEditorAttribute() : base("ckeditor")
        {
        }

        public override OpenApiObject GetCustomOpenApiConfig(LinkGenerator linkGenerator)
        {
            var obj = new OpenApiObject
            {
                ["imageUploadUrl"] = new OpenApiString(
                    // ReSharper disable once Mvc.ActionNotResolved
                    // ReSharper disable once Mvc.ControllerNotResolved
                    $"{Env.Get("ROUTE_PREFIX")}{linkGenerator.GetPathByAction("UploadCkEditor", "FilesUpload")?[1..]}"),
            };

            return obj;
        }
    }
}