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
            await OnPatching(id, eDoc);
            var e = await Repo.Patch(id, eDoc, ServiceProvider.GetRequiredService<IAdapterFactory>());
            await OnPatched(e);
            return Ok(await GetPatchResponseModel(e));
        }

        [HttpPost]
        [ModelValidation]
        public virtual async Task<ActionResult<ModelResponse<TFm>>> Create([FromBody] TFm fm)
        {
            var e = MapF(fm);
            await OnCreating(e);
            e = await Repo.Add(e);
            await OnCreated(e);
            return Ok(await GetCreateResponseModel(e));
        }

        protected virtual TFm MapF(TE e)
        {
            return Mapper.Map<TFm>(e);
        }

        protected virtual TE MapF(TFm vm)
        {
            return Mapper.Map<TE>(vm);
        }


        #region Hooks

        protected virtual Task OnCreating(TE e)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnCreated(TE e)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnPatching(string id, JsonPatchDocument<TE> patchDoc)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnPatched(TE e)
        {
            return Task.CompletedTask;
        }

        #endregion

        protected virtual Task<ModelResponse<TFm>> GetPatchResponseModel(TE e)
        {
            var fm = MapF(e);
            return Task.FromResult(new ModelResponse<TFm>(fm, e.Id));
        }

        protected virtual Task<ModelResponse<TFm>> GetCreateResponseModel(TE e)
        {
            var fm = MapF(e);
            return Task.FromResult(new ModelResponse<TFm>(fm, e.Id));
        }
    }
}