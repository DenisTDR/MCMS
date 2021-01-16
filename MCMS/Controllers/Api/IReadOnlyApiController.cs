using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Data.ViewModels;
using MCMS.Models.Dt;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    public interface IReadOnlyApiController<TVm> where TVm : IViewModel
    {
        Task<ActionResult<List<TVm>>> Index();
        Task<ActionResult<DtResult<TVm>>> DtQuery([FromBody] DtParameters model);
        Task<ActionResult<TVm>> Preview(string id);
    }
}