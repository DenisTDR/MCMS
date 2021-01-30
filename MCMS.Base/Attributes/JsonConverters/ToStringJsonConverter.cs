using System;
using System.Collections;
using System.Collections.Generic;
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

            var str = "[";
            var sList = new List<string>();
            foreach (var o in list)
            {
                sList.Add(GetToStringValue(o));
            }

            return "[" + string.Join(", ", sList) + "]";
        }
    }
}