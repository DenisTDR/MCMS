using System;
using System.Collections;
using System.Collections.Generic;
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

            if (obj is string str)
            {
                return str;
            }

            if (obj is IDictionary dict)
            {
                return "{ " + string.Join(", ",
                    from object key in dict.Keys select key + ": " + GetToStringValue(dict[key])) + " }";
            }

            if (obj is IEnumerable list)
            {
                return "[" + string.Join(", ", from object o in list select GetToStringValue(o)) + "]";
            }


            return obj.ToString();
        }
    }
}