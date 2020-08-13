using System;
using MCMS.Base.Exceptions;
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.ValueType != typeof(string))
            {
                throw new KnownException($"Can't convert '{reader.ValueType.Name}' to '{objectType.Name}'.");
            }

            var str = reader.Value as string;

            foreach (Enum value in Enum.GetValues(objectType))
            {
                if (value.GetDisplayName() == str)
                {
                    return value;
                }
            }

            return existingValue;
        }
    }
}