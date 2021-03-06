using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;

namespace MCMS.Base.Attributes.JsonConverters
{
    public class ToStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(GetToStringValue(value));
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) => throw new NotImplementedException();

        public static string GetToStringValue(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (!(obj is IList list))
            {
                return obj.ToString();
            }

            return "[" + string.Join(", ", from object o in list select GetToStringValue(o)) + "]";
        }
    }
}