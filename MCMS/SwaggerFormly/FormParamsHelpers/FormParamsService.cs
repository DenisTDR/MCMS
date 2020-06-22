using MCMS.Helpers;
using MCMS.SwaggerFormly.Controllers;
using MCMS.SwaggerFormly.Models;

namespace MCMS.SwaggerFormly.FormParamsHelpers
{
    public class FormParamsService
    {
        protected string ControllerPath { get; set; }
        protected string SchemaName { get; set; }
        protected static string ApiUrl { get; set; } = Utils.UrlCombine(Env.GetOrThrow("EXTERNAL_URL"), "api/");
        protected object AdditionalData { get; set; }
        protected object Options { get; set; }
        
        
        public FormParamsService(string controllerPath, string schemaName)
        {
            ControllerPath = controllerPath;
            SchemaName = schemaName;
        }

        public virtual FormlyFormParams ForPatch(string id)
        {
            var @params = CommonConfig(FormActionType.Patch);
            @params.ModelId = id;
            return @params;
        }

        public virtual FormlyFormParams ForCreate(object additionalData = null, object options = null)
        {
            AdditionalData = additionalData;
            Options = options;
            return CommonConfig(FormActionType.Create);
        }

        protected virtual FormlyFormParams CommonConfig(FormActionType action)
        {
            var submitActionName = GetActionName(action);
            var controllerUrl = Utils.UrlCombine(ApiUrl, ControllerPath);
            return new FormlyFormParams
            {
                SchemaName = SchemaName,
                Action = action,
                GetUrl = Utils.UrlCombine(controllerUrl, GetGetActionName() + "/"),
                SubmitUrl = Utils.UrlCombine(controllerUrl, submitActionName + "/"),
                OpenApiConfigUrl = GetOpenApiConfigUrl(),
                FormInstanceId = Utils.GenerateRandomHexString(),
                AdditionalFields = AdditionalData,
                Options = Options
            };
        }

        public static string GetOpenApiConfigUrl()
        {
            var openApiConfigController = nameof(OpenApiConfigController).Replace("Controller", "");
            var openApiControllerUrl = Utils.UrlCombine(ApiUrl, openApiConfigController);
            return Utils.UrlCombine(openApiControllerUrl, nameof(OpenApiConfigController.Get));
        }

        protected virtual string GetActionName(FormActionType actionType)
        {
            return actionType == FormActionType.Create ? "Create" : "Patch";
        }

        protected virtual string GetGetActionName()
        {
            return "Get";
        }
    }
}