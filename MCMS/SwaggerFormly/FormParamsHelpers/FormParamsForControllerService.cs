using MCMS.Base.Data.ViewModels;
using MCMS.Controllers;
using MCMS.Controllers.Api;
using MCMS.SwaggerFormly.Models;

namespace MCMS.SwaggerFormly.FormParamsHelpers
{
    public class FormParamsForControllerService<TController, TForm> : FormParamsService
        where TForm : class, IFormModel, IViewModel
        where TController : IPatchCreateApiController<TForm>
    {
        public FormParamsForControllerService() : base(
            typeof(TController).Name.Replace("Controller", ""), typeof(TForm).Name)
        {
        }
    }
}