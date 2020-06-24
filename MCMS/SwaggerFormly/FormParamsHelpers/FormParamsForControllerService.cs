using MCMS.Base.Data.ViewModels;
using MCMS.Controllers.Api;
using MCMS.Helpers;
using MCMS.SwaggerFormly.Models;

namespace MCMS.SwaggerFormly.FormParamsHelpers
{
    public class FormParamsForControllerService<TController, TFormModel> : FormParamsService
        where TFormModel : class, IFormModel
        where TController : IPatchCreateApiController<TFormModel>
    {
        public FormParamsForControllerService() : base(TypeHelpers.GetControllerName(typeof(TController)),
            typeof(TFormModel).Name)
        {
        }
    }
}