using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<string, JObject> _cache = new();
        private readonly object _cacheLock = new();

        public SwaggerConfigService(ISwaggerProvider swaggerProvider)
        {
            _swaggerProvider = swaggerProvider;
        }

        public JObject GetConfig(string name = "admin-api")
        {
            EnsureCache(name);
            return _cache[name];
        }

        public void Load(string name = "admin-api")
        {
            EnsureCache(name);
        }

        private void EnsureCache(string name)
        {
            if (_cache.ContainsKey(name)) return;
            lock (_cacheLock)
            {
                if (_cache.ContainsKey(name)) return;
                var doc = _swaggerProvider.GetSwagger(name, null, "/");
                doc.Paths.Clear();
                var docJson = doc.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
                var docParsed = JsonConvert.DeserializeObject<JObject>(docJson);
                docParsed.Add("name", JToken.FromObject(name));
                _cache[name] = docParsed;
            }
        }

        public void ClearTranslationsFromCache()
        {
            foreach (var cacheKey in _cache.Keys)
            {
                if (_cache.TryGetValue(cacheKey, out var doc))
                {
                    doc?.Remove("translations");
                }
            }
        }
    }
}