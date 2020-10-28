using MCMS.SwaggerFormly.Models;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Swagger;

namespace MCMS.SwaggerFormly
{
    public class SwaggerConfigService
    {
        private readonly ISwaggerProvider _swaggerProvider;
        private readonly SwaggerConfigOptions _options;
        private JObject _cache;
        private readonly object _cacheLock = new object();

        public SwaggerConfigService(
            ISwaggerProvider swaggerProvider,
            IOptions<SwaggerConfigOptions> options
        )
        {
            _swaggerProvider = swaggerProvider;
            _options = options.Value;
        }

        public JObject GetConfig()
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
            if (_cache != null) return;
            lock (_cacheLock)
            {
                if (_cache != null) return;
                var doc = _swaggerProvider.GetSwagger(_options.Name, null, "/");
                doc.Paths.Clear();
                var docJson = doc.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
                var docParsed = JsonConvert.DeserializeObject<JObject>(docJson);
                _cache = docParsed;
            }
        }

        public void ClearTranslationsFromCache()
        {
            _cache?.Remove("translations");
        }
    }
}