using MCMS.Base.Data.ViewModels;
using MCMS.Controllers;
using MCMS.Helpers;
using MCMS.SwaggerFormly.Controllers;
using MCMS.SwaggerFormly.Models;

namespace MCMS.SwaggerFormly
{
    public class FormParamsService<TController, TForm> where TForm : class, IFormModel, IViewModel
        where TController : IPatchCreateApiController<TForm>
    {
        public FormlyFormParams ForPatch(string id = null)
        {
            var @params = CommonConfig(FormActionType.Create);
            @params.Id = id;
            return @params;
        }

        public FormlyFormParams ForCreate()
        {
            return CommonConfig(FormActionType.Create);
        }


        private FormlyFormParams CommonConfig(FormActionType action)
        {
            var apiUrl = Utilis.UrlCombine(Env.GetOrThrow("EXTERNAL_URL"), "api/");
            var urlBase = Utilis.UrlCombine(apiUrl, typeof(TController).Name.Replace("Controller", ""));

            var submitActionName = action == FormActionType.Create
                ? nameof(IPatchCreateApiController<TForm>.Create)
                : nameof(IPatchCreateApiController<TForm>.Patch);

            var openApiConfigController = nameof(OpenApiConfigController).Replace("Controller", "");
            var openApiControllerUrl = Utilis.UrlCombine(apiUrl, openApiConfigController);
            return new FormlyFormParams
            {
                SchemaName = typeof(TForm).Name,
                Action = action,
                GetUrl = Utilis.UrlCombine(urlBase, nameof(IPatchCreateApiController<TForm>.Get)),
                SubmitUrl = Utilis.UrlCombine(urlBase, submitActionName + "/"),
                OpenApiConfigUrl = Utilis.UrlCombine(openApiControllerUrl, nameof(OpenApiConfigController.Get)),
            };
        }
    }
}