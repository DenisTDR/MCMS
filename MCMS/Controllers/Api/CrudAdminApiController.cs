using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Repositories;
using MCMS.Data;
using MCMS.Models;
using MCMS.Models.Dt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers.Api
{
    public class CrudAdminApiController<TE, TFm, TVm> : PatchCreateAdminApiController<TE, TFm>,
        ICrudAdminApiController<TFm, TVm>
        where TE : class, IEntity, new() where TFm : class, IFormModel where TVm : class, IViewModel
    {
        protected virtual DtQueryService<TVm> QueryService => ServiceProvider.GetService<DtQueryService<TVm>>();

        [AdminApiRoute("~/[controller]")]
        [HttpGet]
        public virtual async Task<ActionResult<List<TVm>>> Index()
        {
            var all = await Repo.GetAll();
            var allVm = Map(all);
            return Ok(allVm);
        }

        [AdminApiRoute("~/[controller]/dtquery")]
        [HttpPost]
        [ModelValidation]
        [ReadOnlyApi]
        public virtual async Task<ActionResult<DtResult<TVm>>> DtQuery([FromBody] [Required] DtParameters model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await QueryService.Query(Repo, model);
            return Ok(result);
        }

        [AdminApiRoute("~/[controller]/light")]
        [HttpGet]
        public virtual Task<ActionResult<List<TVm>>> IndexLight()
        {
            Repo.ChainQueryable(q => q.Select(e => new TE {Id = e.Id}));
            return Index();
        }

        [Route("{id}")]
        [HttpGet]
        public virtual async Task<ActionResult<TVm>> Preview([FromRoute] string id)
        {
            var e = await Repo.GetOneOrThrow(id);
            var fm = MapV(e);
            return Ok(fm);
        }


        [HttpDelete]
        [AdminApiRoute("~/[controller]/{id}")]
        public virtual async Task<ActionResult<string>> Delete([FromRoute] string id)
        {
            await Repo.Delete(id);
            return Ok(id);
        }

        [HttpDelete]
        public virtual async Task<ActionResult<List<string>>> BatchDelete([FromQuery] List<string> ids)
        {
            await Repo.Delete(ids);
            return Ok(ids);
        }

        protected virtual List<TVm> Map(List<TE> entities)
        {
            return Mapper.Map<List<TVm>>(entities);
        }

        protected virtual TVm MapV(TE e)
        {
            return Mapper.Map<TVm>(e);
        }

        protected override async Task<ModelResponse<TFm>> GetPatchResponseModel(TE e)
        {
            var fm = MapF(e);
            var vm = MapV(e);
            var response = new FormSubmitResponse<TFm, TVm>(fm, vm, e.Id)
            {
                Snack = await ServiceProvider.GetRequiredService<ITranslationsRepository>().GetValueOrSlug("updated"),
                SnackType = "success",
                SnackDuration = 3000
            };
            return response;
        }

        protected override async Task<ModelResponse<TFm>> GetCreateResponseModel(TE e)
        {
            var fm = MapF(e);
            var vm = MapV(e);
            var response = new FormSubmitResponse<TFm, TVm>(fm, vm, e.Id)
            {
                Snack = await ServiceProvider.GetRequiredService<ITranslationsRepository>().GetValueOrSlug("saved"),
                SnackType = "success",
                SnackDuration = 3000
            };
            return response;
        }
    }
}