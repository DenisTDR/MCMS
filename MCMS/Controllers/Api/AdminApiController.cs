using System;
using AutoMapper;
using MCMS.Base.Attributes;
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
    }
}