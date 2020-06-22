using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Attributes;
using MCMS.Base.Data.Entities;
using MCMS.Base.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    public class GenericApiController<TE, TVm> : PatchCreateApiController<TE, TVm>
        where TE : Entity where TVm : class, IViewModel
    {
        [ApiRoute("/[controller]")]
        [HttpGet]
        public virtual async Task<ActionResult<List<TVm>>> Index()
        {
            var all = await Repo.GetAll();
            var allVm = Map(all);
            return Ok(allVm);
        }

        protected List<TVm> Map(List<TE> entities)
        {
            return Mapper.Map<List<TVm>>(entities);
        }
    }
}