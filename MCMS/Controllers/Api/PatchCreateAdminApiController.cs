using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MCMS.Base.Data.Entities;
using MCMS.Base.Extensions;
using MCMS.Data;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers.Api
{
    public abstract class PatchCreateAdminApiController<TE, TFm> : AdminApiController, IPatchCreateApiController<TFm>
        where TE : Entity where TFm : class, IFormModel
    {
        protected IRepository<TE> Repo => ServiceProvider.GetService<IRepository<TE>>();

        [Route("{id}")]
        [HttpGet]
        public virtual async Task<ActionResult<TFm>> Get([FromRoute] string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            var fm = MapF(e);
            return Ok(fm);
        }

        [Route("{id}")]
        [HttpPatch]
        public virtual async Task<ActionResult<TFm>> Patch([FromRoute] [Required] string id,
            [FromBody] [Required] JsonPatchDocument<TFm> doc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await Repo.Any(id))
            {
                return NotFound();
            }

            var eDoc = doc.CloneFor<TFm, TE>();

            var e = await Repo.Patch(id, eDoc);
            var fm = MapF(e);

            return Ok(fm);
        }

        [HttpPost]
        public virtual async Task<ActionResult<TFm>> Create([FromBody] TFm fm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var e = MapF(fm);
            AttachFkProperties(e);
            e = await Repo.Add(e);
            fm = MapF(e);
            return Ok(fm);
        }

        protected virtual TFm MapF(TE e)
        {
            return Mapper.Map<TFm>(e);
        }

        protected virtual TE MapF(TFm vm)
        {
            return Mapper.Map<TE>(vm);
        }

        protected virtual void AttachFkProperties(TE e)
        {
        }
    }
}