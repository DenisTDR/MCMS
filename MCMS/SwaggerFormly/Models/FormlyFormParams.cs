using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCMS.Base.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCMS.SwaggerFormly.Models
{
    public class FormlyFormParams
    {
        public string OpenApiConfigUrl { get; set; }
        public string SchemaName { get; set; }
        public FormActionType Action { get; set; }
        public string GetUrl { get; set; }
        public string SubmitUrl { get; set; }
        public object AdditionalFields { get; set; }
        public string ModelId { get; set; }
        public string FormInstanceId { get; set; }
        public Dictionary<string, object> Options { get; set; }


        public string ToUrlQuery()
        {
            var jObj = (JObject) JsonConvert.DeserializeObject(JsonConvert.SerializeObject(this,
                Utils.DefaultJsonSerializerSettings()));
            return string.Join("&",
                jObj.Properties().Where(jProp => jProp.Value != null && !string.IsNullOrEmpty(jProp.Value.ToString()))
                    .Select(jProp => jProp.Name + "=" + HttpUtility.UrlEncode(JsonConvert.SerializeObject(jProp.Value, Utils.DefaultJsonSerializerSettings()))));
        }
    }

    public enum FormActionType
    {
        [EnumMember(Value = "create")] Create,
        [EnumMember(Value = "patch")] Patch
    }
}