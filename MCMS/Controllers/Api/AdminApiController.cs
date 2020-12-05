using MCMS.Base.Attributes;
using MCMS.Base.Controllers;
using MCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    [Authorize]
    [AdminApiRoute("[controller]/[action]")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "admin-api")]
    public class AdminApiController : BaseController
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