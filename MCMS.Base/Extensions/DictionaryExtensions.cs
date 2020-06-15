using System.Collections.Generic;

namespace MCMS.Base.Extensions
{
    public static class DictionaryExtensions
    {
        // public static T GetOrSetDefault<T>(this IDictionary<string, T> dict, string key) where T : new()
        // {
        //     if (dict.TryGetValue(key, out T value))
        //     {
        //         return value;
        //     }
        //
        //     return dict[key] = new T();
        // }

        public static T1 GetOrSetDefault<T1, T2>(this IDictionary<string, T2> dict, string key,
            T1 defaultValue = null)
            where T1 : class, T2, new()
        {
            if (dict.TryGetValue(key, out T2 value))
            {
                return value as T1;
            }

            return (T1) (dict[key] = defaultValue ?? new T1());
        }
    }
}