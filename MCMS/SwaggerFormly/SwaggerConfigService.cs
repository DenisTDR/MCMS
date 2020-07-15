using MCMS.SwaggerFormly.Models;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace MCMS.SwaggerFormly
{
    public class SwaggerConfigService
    {
        private readonly ISwaggerProvider _swaggerProvider;
        private readonly SwaggerConfigOptions _options;
        private object _cache;

        public SwaggerConfigService(ISwaggerProvider swaggerProvider, IOptions<SwaggerConfigOptions> options)
        {
            _swaggerProvider = swaggerProvider;
            _options = options.Value;
        }

        public object GetConfig()
        {
            EnsureCache();
            return _cache;
        }

        public void Load()
        {
            EnsureCache();
        }

        private void EnsureCache()
        {
            if (_cache == null)
            {
                var doc = _swaggerProvider.GetSwagger(_options.Name, null, "/");
                doc.Paths.Clear();
                var docJson = doc.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
                var docParsed = JsonConvert.DeserializeObject(docJson);
                _cache = docParsed;
            }
        }
    }
}