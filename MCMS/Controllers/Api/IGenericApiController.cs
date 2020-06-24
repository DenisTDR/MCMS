using MCMS.Base.Data.ViewModels;
using MCMS.SwaggerFormly.Models;

namespace MCMS.Controllers.Api
{
    public interface IGenericApiController<TFm, TVm> : IPatchCreateApiController<TFm>, IReadOnlyApiController<TVm>
        where TFm : class, IFormModel
        where TVm : class, IViewModel
    {
    }
}