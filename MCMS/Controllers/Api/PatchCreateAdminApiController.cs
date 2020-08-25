using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Data;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Extensions;
using MCMS.Base.JsonPatch;
using MCMS.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers.Api
{
    public abstract class PatchCreateAdminApiController<TE, TFm> : AdminApiController, IPatchCreateApiController<TFm>
        where TE : class, IEntity where TFm : class, IFormModel
    {
        protected virtual IRepository<TE> Repo => ServiceProvider.GetRepo<TE>();

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
        [PatchDocumentValidation]
        public virtual async Task<ActionResult<ModelResponse<TFm>>> Patch([FromRoute] [Required] string id,
            [FromBody] [Required] JsonPatchDocument<TFm> doc)
        {
            if (!await Repo.Any(id))
            {
                return NotFound();
            }

            var eDoc = doc.CloneFor<TFm, TE>();

            var e = await Repo.Patch(id, eDoc, ServiceProvider.GetService<IAdapterFactory>());
            var fm = MapF(e);

            return OkModel(fm);
        }

        [HttpPost]
        [ModelValidation]
        public virtual async Task<ActionResult<ModelResponse<TFm>>> Create([FromBody] TFm fm)
        {
            var e = MapF(fm);
            await BeforeSaveNewHook(e);
            e = await Repo.Add(e);
            await AfterSaveNewHook(e);
            fm = MapF(e);
            return OkModel(fm);
        }

        protected virtual TFm MapF(TE e)
        {
            return Mapper.Map<TFm>(e);
        }

        protected virtual TE MapF(TFm vm)
        {
            return Mapper.Map<TE>(vm);
        }

        protected virtual Task BeforeSaveNewHook(TE e)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterSaveNewHook(TE e)
        {
            return Task.CompletedTask;
        }
    }
}