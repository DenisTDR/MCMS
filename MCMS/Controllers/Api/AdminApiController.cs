using System;
using AutoMapper;
using MCMS.Base.Attributes;
using MCMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Controllers.Api
{
    [AdminApiRoute("[controller]/[action]")]
    [Produces("application/json")]
    [Authorize]
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