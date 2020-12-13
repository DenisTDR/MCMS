using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    public interface IDeleteApiController
    {
        Task<ActionResult<string>> Delete([FromBody] string id);
        Task<ActionResult<List<string>>> BatchDelete([FromBody] List<string> id);
    }
}