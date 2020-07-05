using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;

namespace MCMS.Controllers.Api
{
    public interface IGenericApiController<TFm, TVm> : IPatchCreateApiController<TFm>, IReadOnlyApiController<TVm>
        where TFm : class, IFormModel
        where TVm : class, IViewModel
    {
    }
}