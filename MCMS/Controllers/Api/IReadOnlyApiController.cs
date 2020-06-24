using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    public interface IReadOnlyApiController<TVm> where TVm : IViewModel
    {
        Task<ActionResult<List<TVm>>> Index();
        Task<ActionResult<TVm>> Preview(string id);
    }
}