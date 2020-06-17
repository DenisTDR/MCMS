using System.Threading.Tasks;
using MCMS.Base.SwaggerFormly;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using MCMS.Controllers;
using MCMS.SwaggerFormly.Models;

namespace MCMS.SwaggerFormly.Controllers
{
    public class OpenApiConfigController : ApiController
    {
        private readonly ISwaggerProvider _swaggerProvider;
        private readonly SwaggerConfigOptions _options;

        public OpenApiConfigController(
            ISwaggerProvider swaggerProvider,
            IOptions<SwaggerConfigOptions> options
        )
        {
            _swaggerProvider = swaggerProvider;
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var doc = _swaggerProvider.GetSwagger(_options.Name, null, "/");
            var docJson = doc.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
            var docParsed = JsonConvert.DeserializeObject(docJson);
            return Ok(docParsed);
        }

        [HttpGet]
        public async Task<IActionResult> GetDelayed()
        {
            await Task.Delay(1000);
            var doc = _swaggerProvider.GetSwagger(_options.Name, null, "/");
            var docJson = doc.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
            var docParsed = JsonConvert.DeserializeObject(docJson);
            return Ok(docParsed);
        }
    }
}