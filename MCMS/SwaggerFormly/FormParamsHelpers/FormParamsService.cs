using MCMS.Helpers;
using MCMS.SwaggerFormly.Controllers;
using MCMS.SwaggerFormly.Models;

namespace MCMS.SwaggerFormly.FormParamsHelpers
{
    public class FormParamsService
    {
        protected string ControllerPath { get; set; }
        protected string SchemaName { get; set; }
        protected string ApiUrl { get; set; } = Utilis.UrlCombine(Env.GetOrThrow("EXTERNAL_URL"), "api/");

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

        public virtual FormlyFormParams ForCreate()
        {
            return CommonConfig(FormActionType.Create);
        }

        protected virtual FormlyFormParams CommonConfig(FormActionType action)
        {
            var submitActionName = GetActionName(action);

            var openApiConfigController = nameof(OpenApiConfigController).Replace("Controller", "");
            var openApiControllerUrl = Utilis.UrlCombine(ApiUrl, openApiConfigController);

            var controllerUrl = Utilis.UrlCombine(ApiUrl, ControllerPath);

            return new FormlyFormParams
            {
                SchemaName = SchemaName,
                Action = action,
                GetUrl = Utilis.UrlCombine(controllerUrl, GetGetActionName() + "/"),
                SubmitUrl = Utilis.UrlCombine(controllerUrl, submitActionName + "/"),
                OpenApiConfigUrl = Utilis.UrlCombine(openApiControllerUrl, nameof(OpenApiConfigController.Get)),
                FormInstanceId = Utilis.GenerateRandomHexString()
            };
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