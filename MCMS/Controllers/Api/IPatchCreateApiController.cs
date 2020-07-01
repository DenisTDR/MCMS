using System.Threading.Tasks;
using MCMS.SwaggerFormly.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    public interface IPatchCreateApiController<TFm> where TFm : class, IFormModel
    {
        Task<ActionResult<TFm>> Get([FromRoute] string id);
        Task<ActionResult<TFm>> Patch([FromQuery] string id, [FromBody] JsonPatchDocument<TFm> bag);
        Task<ActionResult<TFm>> Create([FromBody] TFm model);
    }
}