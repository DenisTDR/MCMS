using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            return obj switch
            {
                null => null,
                string str => str,
                JValue jValue => jValue.ToString(CultureInfo.InvariantCulture),
                IDictionary dict => "{ " + string.Join(", ",
                    from object key in dict.Keys select key + ": " + GetToStringValue(dict[key])) + " }",
                IEnumerable<string> listStr => "[" + string.Join(", ", listStr) + "]",
                IEnumerable list => "[" + string.Join(", ", from object o in list select GetToStringValue(o)) + "]",
                DateTime dateTime => dateTime.ToString("u"),
                _ => obj.ToString()
            };
        }
    }
}