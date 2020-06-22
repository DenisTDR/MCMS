using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MCMS.Attributes;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Extensions;
using MCMS.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers.Api
{
    public abstract class PatchCreateApiController<TE, TVm> : ApiController, IPatchCreateApiController<TVm>
        where TE : Entity where TVm : class, IViewModel
    {
        protected IRepository<TE> Repo => ServiceProvider.GetService<IRepository<TE>>();

        [Route("{id}")]
        [HttpGet]
        public virtual async Task<ActionResult<TVm>> Get([FromRoute] string id)
        {
            var e = await Repo.GetOne(id);
            if (e == null)
            {
                return NotFound();
            }

            var vm = Map(e);
            return Ok(vm);
        }

        [Route("{id}")]
        [HttpPatch]
        public virtual async Task<ActionResult<TVm>> Patch([FromRoute] [Required] string id,
            [FromBody] [Required] JsonPatchDocument<TVm> doc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await Repo.Any(id))
            {
                return NotFound();
            }

            var eDoc = doc.CloneFor<TVm, TE>();

            var e = await Repo.Patch(id, eDoc);
            var vm = Map(e);

            return Ok(vm);
        }

        [HttpPost]
        public virtual async Task<ActionResult<TVm>> Create([FromBody] TVm vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var e = Map(vm);
            e = await Repo.Add(e);
            vm = Map(e);
            return Ok(vm);
        }

        protected TVm Map(TE e)
        {
            return Mapper.Map<TVm>(e);
        }

        protected TE Map(TVm vm)
        {
            return Mapper.Map<TE>(vm);
        }
    }
}