using MCMS.Base.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers.Api
{
    [ApiRoute("[controller]/[action]")]
    [Produces("application/json")]
    public class ApiController : BaseController
    {
    }
}