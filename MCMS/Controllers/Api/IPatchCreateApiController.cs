using System.Threading.Tasks;
using MCMS.Base.Data.FormModels;
using MCMS.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    public interface IPatchCreateApiController<TFm> where TFm : class, IFormModel
    {
        Task<ActionResult<TFm>> Get([FromRoute] string id);
        Task<ActionResult<ModelResponse<TFm>>> Patch([FromQuery] string id, [FromBody] JsonPatchDocument<TFm> bag);
        Task<ActionResult<ModelResponse<TFm>>> Create([FromBody] TFm model);
    }
}