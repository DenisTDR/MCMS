using System;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;

namespace MCMS.Files.Attributes
{
    public class FormlyFileAttribute : FormlyFieldAttribute
    {
        public Type FileUploadControllerType { get; }
        public string FileUploadActionName { get; }
        public string FileDeleteActionName { get; }
        public bool Multiple { get; set; }

        public FormlyFileAttribute(Type fileUploadControllerType, string fileUploadActionName,
            string fileDeleteActionName = "Delete")
        {
            FileUploadControllerType = fileUploadControllerType;
            FileUploadActionName = fileUploadActionName;
            FileDeleteActionName = fileDeleteActionName;
            Type = "file";
            AsOpenApi = true;
        }

        public override OpenApiObject GetOpenApiConfig(LinkGenerator linkGenerator)
        {
            var obj = new OpenApiObject
            {
                ["uploadUrl"] = new OpenApiString(linkGenerator.GetAbsolutePathByAction(FileUploadActionName,
                    TypeHelpers.GetControllerName(FileUploadControllerType))),
                ["deleteUrl"] = new OpenApiString(linkGenerator.GetAbsolutePathByAction(FileDeleteActionName,
                    TypeHelpers.GetControllerName(FileUploadControllerType))),
                ["multiple"] = new OpenApiBoolean(Multiple)
            };
            return obj;
        }
    }
}