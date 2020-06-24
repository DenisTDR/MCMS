using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    public class GenericApiController<TE, TFm, TVm> : PatchCreateApiController<TE, TFm>, IGenericApiController<TFm, TVm>
        where TE : Entity where TFm : class, IFormModel where TVm : class, IViewModel
    {
        [ApiRoute("/[controller]")]
        [HttpGet]
        public virtual async Task<ActionResult<List<TVm>>> Index()
        {
            var all = await Repo.GetAll();
            var allVm = Map(all);
            return Ok(allVm);
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
    }
}