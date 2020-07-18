using System;
using System.Linq;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Base.SwaggerFormly.Formly.Fields;
using Microsoft.AspNetCore.Routing;
using Microsoft.OpenApi.Any;

namespace MCMS.Files.Attributes
{
    public class FormlyFileAttribute : FormlyFieldAttribute
    {
        private string[] _accept;
        public Type FileUploadControllerType { get; }
        public string FileUploadActionName { get; }
        public string Purpose { get; }
        public string Path { get; }
        public bool Private { get; set; }
        public string FileDeleteActionName { get; set; } = "Delete";
        public bool Multiple { get; set; }

        public string[] Accept
        {
            get => _accept;
            set => _accept = value?.Select(ext => ext.StartsWith(".") ? ext : "." + ext)
                .Select(ext => ext.ToLower()).OrderBy(ext => ext).ToArray();
        }

        public string AcceptStr => string.Join(",", _accept ?? new string[] { });

        public FormlyFileAttribute(Type fileUploadControllerType, string fileUploadActionName, string purpose,
            string path = null)
        {
            FileUploadControllerType = fileUploadControllerType;
            FileUploadActionName = fileUploadActionName;
            Purpose = purpose;
            Path = path;
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
                ["multiple"] = new OpenApiBoolean(Multiple),
                ["purpose"] = new OpenApiString(Purpose),
                ["accept"] = new OpenApiString(AcceptStr),
            };
            return obj;
        }
    }
}