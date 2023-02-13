using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MCMS.Base.Helpers
{
    public static class Utils
    {
        public static string GenerateRandomHexString(int length = 20)
        {
            var str = "";
            while (str.Length < length)
            {
                str += Guid.NewGuid().ToString().ToLower().Replace("-", "");
            }

            return str.Substring(0, length);
        }

        private static readonly Random Random = new();
        private const string LowerCaseAlphaNumeric = "abcdefghijklmnopqrstuvwxyz0123456789";
        private const string UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GenerateRandomString(int length = 20, bool useUpperCase = false)
        {
            var src = useUpperCase ? LowerCaseAlphaNumeric + UpperCaseLetters : LowerCaseAlphaNumeric;
            return new(Enumerable
                .Repeat(src, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static void DieWith(string message, bool killProcess = true, ConsoleColor color = ConsoleColor.Red)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
            if (killProcess)
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        public static string UrlCombine(params string[] p)
        {
            return Path.Combine(p).Replace("\\", "/");
        }

        private static JsonSerializerSettings indentedSerializerSettings;
        private static JsonSerializerSettings serializerSettings;

        public static JsonSerializerSettings DefaultJsonSerializerSettings(bool indented = false)
        {
            return indented
                ? indentedSerializerSettings ??= new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter(new CamelCaseNamingStrategy())
                    },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Formatting = Formatting.Indented,
                }
                : serializerSettings ??= new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter(new CamelCaseNamingStrategy())
                    },
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                };
        }

        public static string Serialize(object obj, bool indented = true)
        {
            return JsonConvert.SerializeObject(obj, DefaultJsonSerializerSettings(indented));
        }
    }
}