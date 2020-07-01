using MCMS.Base.Helpers;
using MCMS.Controllers.Api;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.SwaggerFormly.FormParamsHelpers
{
    public class FormParamsForControllerService<TController, TFormModel> : FormParamsService
        where TFormModel : class, IFormModel
        where TController : IPatchCreateApiController<TFormModel>
    {
        public FormParamsForControllerService(IUrlHelper urlHelper) : base(urlHelper,
            TypeHelpers.GetControllerName(typeof(TController)), typeof(TFormModel).Name)
        {
        }
    }
}