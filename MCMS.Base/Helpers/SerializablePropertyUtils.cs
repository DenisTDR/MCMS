using Newtonsoft.Json;

namespace MCMS.Base.Helpers
{
    public static class SerializablePropertyUtils
    {
        public static string SerializeOrEmpty<T>(T obj, JsonSerializerSettings settings = null) where T : new()
        {
            settings ??= Utils.DefaultJsonSerializerSettings();

            return JsonConvert.SerializeObject(obj ?? new T(), settings);
        }

        public static T DeserializeOrDefault<T>(string serializedData, bool nullOnDefault = false)
            where T : class, new()
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                return nullOnDefault ? null : new T();
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(serializedData);
            }
            catch
            {
                return new T();
            }
        }
    }
}