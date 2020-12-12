using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.FormModels;
using MCMS.Base.Data.ViewModels;
using MCMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    public class GenericAdminApiController<TE, TFm, TVm> : PatchCreateAdminApiController<TE, TFm>,
        IGenericApiController<TFm, TVm>
        where TE : class, IEntity, new() where TFm : class, IFormModel where TVm : class, IViewModel
    {
        [AdminApiRoute("~/[controller]")]
        [HttpGet]
        public virtual async Task<ActionResult<List<TVm>>> Index()
        {
            var all = await Repo.GetAll();
            var allVm = Map(all);
            return Ok(allVm);
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

        protected virtual List<TVm> Map(List<TE> entities)
        {
            return Mapper.Map<List<TVm>>(entities);
        }

        protected virtual TVm MapV(TE e)
        {
            return Mapper.Map<TVm>(e);
        }

        protected override Task<ModelResponse<TFm>> GetPatchResponseModel(TE e)
        {
            var fm = MapF(e);
            var vm = MapV(e);
            return Task.FromResult(new DoubleModelResponse<TFm, TVm>(fm, vm, e.Id) as ModelResponse<TFm>);
        }

        protected override Task<ModelResponse<TFm>> GetCreateResponseModel(TE e)
        {
            var fm = MapF(e);
            var vm = MapV(e);
            return Task.FromResult(new DoubleModelResponse<TFm, TVm>(fm, vm, e.Id) as ModelResponse<TFm>);
        }
    }
}