using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MCMS.Base.SwaggerFormly
{
    public static class FormsUtils
    {
        private static JsonSerializerSettings _serializerSettings;

        public static JsonSerializerSettings DefaultJsonSerializerSettings()
        {
            return _serializerSettings ??= new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter(new CamelCaseNamingStrategy())
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }
    }
}