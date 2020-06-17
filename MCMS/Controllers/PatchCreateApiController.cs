using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers
{
    public abstract class PatchCreateApiController<TE, TVm> : ApiController, IPatchCreateApiController<TVm>
        where TE : IEntity where TVm : class, IViewModel
    {
        [HttpGet("{id}")]
        public virtual Task<ActionResult<TVm>> Get([FromRoute] string id)
        {
            throw new System.NotImplementedException();
        }

        [HttpPatch("{id}")]
        public virtual Task<ActionResult<TVm>> Patch([FromQuery] string id, [FromBody] JsonPatchDocument<TVm> doc)
        {
            throw new System.NotImplementedException();
        }

        [HttpPost]
        public virtual Task<ActionResult<TVm>> Create([FromBody] TVm model)
        {
            throw new System.NotImplementedException();
        }
    }
}