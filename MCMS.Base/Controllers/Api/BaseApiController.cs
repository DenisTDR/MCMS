using MCMS.Base.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Base.Controllers.Api
{
    [ApiRoute("[controller]/[action]")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "api")]
    public class BaseApiController : BaseController
    {
    }
}