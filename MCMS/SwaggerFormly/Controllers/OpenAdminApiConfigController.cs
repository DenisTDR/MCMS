using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MCMS.Controllers.Api;

namespace MCMS.SwaggerFormly.Controllers
{
    public class OpenApiConfigController : AdminApiController
    {
        private readonly SwaggerConfigService _swaggerConfigService;

        public OpenApiConfigController(
            SwaggerConfigService swaggerConfigService
        )
        {
            _swaggerConfigService = swaggerConfigService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_swaggerConfigService.GetConfig());
        }

        [HttpGet]
        public async Task<IActionResult> GetDelayed()
        {
            await Task.Delay(1000);
            return Ok(_swaggerConfigService.GetConfig());
        }
    }
}