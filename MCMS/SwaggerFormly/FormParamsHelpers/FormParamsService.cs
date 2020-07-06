using MCMS.Base.Helpers;
using MCMS.SwaggerFormly.Controllers;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.SwaggerFormly.FormParamsHelpers
{
    public class FormParamsService
    {
        public IUrlHelper UrlHelper { get; set; }
        protected string ControllerName { get; set; }
        protected string SchemaName { get; set; }
        protected object AdditionalData { get; set; }
        protected object Options { get; set; }


        public FormParamsService(IUrlHelper urlHelper, string controllerName, string schemaName)
        {
            UrlHelper = urlHelper;
            ControllerName = controllerName;
            SchemaName = schemaName;
        }

        public virtual FormlyFormParams ForPatch(string id)
        {
            var config = CommonConfig(FormActionType.Patch);
            config.GetUrl = GetUrlFor(ControllerName, GetGetActionName(), new {id});
            config.SubmitUrl = UrlHelper.ActionLink(GetActionName(FormActionType.Patch), ControllerName, new {id}, protocol: Utils.GetExternalProtocol());
            config.ModelId = id;
            return config;
        }

        public virtual FormlyFormParams ForCreate(object additionalData = null, object options = null)
        {
            AdditionalData = additionalData;
            Options = options;
            var config = CommonConfig(FormActionType.Create);
            config.SubmitUrl = UrlHelper.ActionLink(GetActionName(FormActionType.Create), ControllerName, protocol: Utils.GetExternalProtocol());
            return config;
        }

        protected virtual FormlyFormParams CommonConfig(FormActionType action)
        {
            return new FormlyFormParams
            {
                SchemaName = SchemaName,
                Action = action,
                OpenApiConfigUrl = GetOpenApiConfigUrl(UrlHelper),
                FormInstanceId = Utils.GenerateRandomHexString(),
                AdditionalFields = AdditionalData,
                Options = Options
            };
        }

        public static string GetOpenApiConfigUrl(IUrlHelper urlHelper)
        {
            return urlHelper.ActionLink(nameof(OpenAdminApiConfigController.Get),
                TypeHelpers.GetControllerName(typeof(OpenAdminApiConfigController)), protocol: Utils.GetExternalProtocol());
        }

        protected virtual string GetActionName(FormActionType actionType)
        {
            return actionType == FormActionType.Create ? "Create" : "Patch";
        }

        protected virtual string GetGetActionName()
        {
            return "Get";
        }

        protected virtual string GetUrlFor(string controllerName, string actionName, object values = null)
        {
            var url = UrlHelper.ActionLink(actionName, controllerName, values, protocol: Utils.GetExternalProtocol());
            return url;
        }
    }
}