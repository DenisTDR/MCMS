using System;
using Newtonsoft.Json;

namespace MCMS.Base.Attributes
{
    public class ToStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) => throw new NotImplementedException();
    }
}