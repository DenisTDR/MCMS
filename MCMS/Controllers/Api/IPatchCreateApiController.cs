using System.Threading.Tasks;
using MCMS.Base.Data.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    public interface IPatchCreateApiController<TVm> where TVm : class, IViewModel
    {
        public Task<ActionResult<TVm>> Get([FromRoute] string id);
        public Task<ActionResult<TVm>> Patch([FromQuery] string id, [FromBody] JsonPatchDocument<TVm> bag);
        public Task<ActionResult<TVm>> Create([FromBody] TVm model);
    }
}