using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [Authorize]
    public class ApiController : Controller
    {
    }
}