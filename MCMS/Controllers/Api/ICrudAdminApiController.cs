using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;

namespace MCMS.Controllers.Api
{
    public interface ICrudAdminApiController<TFm, TVm> : IPatchCreateApiController<TFm>, IReadOnlyApiController<TVm>, IDeleteApiController
        where TFm : class, IFormModel
        where TVm : class, IViewModel
    {
    }
}