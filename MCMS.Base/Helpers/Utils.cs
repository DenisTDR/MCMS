using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
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

        public static JsonSerializerSettings DefaultJsonSerializerSettings(bool indented = false)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters =
                    new List<JsonConverter>
                    {
                        new Newtonsoft.Json.Converters.StringEnumConverter()
                    },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }

            return settings;
        }
    }
}