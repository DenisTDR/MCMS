using MCMS.Base.Data.ViewModels;
using MCMS.Controllers.Api;
using MCMS.SwaggerFormly.Models;

namespace MCMS.SwaggerFormly.FormParamsHelpers
{
    public class FormParamsForControllerService<TController, TFormModel> : FormParamsService
        where TFormModel : class, IFormModel
        where TController : IPatchCreateApiController<TFormModel>
    {
        public FormParamsForControllerService() : base(
            typeof(TController).Name.Replace("Controller", ""), typeof(TFormModel).Name)
        {
        }
    }
}