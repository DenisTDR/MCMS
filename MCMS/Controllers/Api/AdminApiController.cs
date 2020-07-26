using MCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{

    [Authorize]
    public class AdminApiController : ApiController
    {
        protected virtual OkObjectResult OkModel<T>(T model)
        {
            return Ok(new ModelResponse<T>(model));
        }

        protected virtual ObjectResult StatusModel<T>(int code, T model)
        {
            return StatusCode(code, new ModelResponse<T>(model));
        }
    }
}