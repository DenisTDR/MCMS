using System;
using MCMS.Base.Extensions;
using Newtonsoft.Json;

namespace MCMS.Base.Attributes.JsonConverters
{
    public class EnumNameJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsEnum;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((value as Enum).GetDisplayName());
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) => throw new NotImplementedException();
    }
}