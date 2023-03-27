using System.Collections.Generic;
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
        public string SchemaName { get; set; }
        protected object AdditionalData { get; set; }
        protected Dictionary<string, object> Options { get; set; }

        public string GetActionName { get; set; }
        public string SubmitActionName { get; set; }

        public string OpenApiConfigName { get; set; } = "admin-api";


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
            config.SubmitUrl =
                UrlHelper.ActionLink(GetSubmitActionName(FormActionType.Patch), ControllerName, new {id});
            config.ModelId = id;
            return config;
        }

        public virtual FormlyFormParams ForCreate(object additionalData = null,
            Dictionary<string, object> options = null)
        {
            AdditionalData = additionalData;
            Options = options;
            var config = CommonConfig(FormActionType.Create);
            config.SubmitUrl = UrlHelper.ActionLink(GetSubmitActionName(FormActionType.Create), ControllerName);
            return config;
        }

        protected virtual FormlyFormParams CommonConfig(FormActionType action)
        {
            var formParams = new FormlyFormParams
            {
                SchemaName = SchemaName,
                Action = action,
                OpenApiConfigUrl = GetOpenApiConfigUrl(UrlHelper, new {name = OpenApiConfigName}),
                FormInstanceId = Utils.GenerateRandomHexString(),
                AdditionalFields = AdditionalData,
                Options = Options
            };
            formParams.AddOption("basePath", UrlHelper.Content("~"));
            if (OpenApiConfigName != "admin-api")
            {
                formParams.AddOption("openApiConfigUrl", formParams.OpenApiConfigUrl);
            }

            formParams.UseFormSpinnerInside();

            return formParams;
        }

        public static string GetOpenApiConfigUrl(IUrlHelper urlHelper, object values = null)
        {
            return urlHelper.ActionLink(nameof(OpenApiConfigController.Get),
                TypeHelpers.GetControllerName(typeof(OpenApiConfigController)), values);
        }

        protected virtual string GetSubmitActionName(FormActionType actionType)
        {
            return SubmitActionName ?? (actionType == FormActionType.Create ? "Create" : "Patch");
        }

        protected virtual string GetGetActionName()
        {
            return GetActionName ?? "Get";
        }

        protected virtual string GetUrlFor(string controllerName, string actionName, object values = null)
        {
            var url = UrlHelper.ActionLink(actionName, controllerName, values);
            return url;
        }
    }
}