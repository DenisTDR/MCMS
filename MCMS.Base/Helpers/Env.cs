using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dotenv.net;

namespace MCMS.Base.Helpers
{
    public static class Env
    {
        private static readonly ConcurrentDictionary<string, string> Cache = new();

        public static string GetOrThrow(string name)
        {
            var value = Get(name);
            if (string.IsNullOrEmpty(value))
            {
                Utils.DieWith("Required Env var '" + name + "' is not set.");
            }

            return value;
        }

        public static string Get(string name)
        {
            if (!Cache.ContainsKey(name))
            {
                var value = Environment.GetEnvironmentVariable(name);
                Cache[name] = value;
            }

            return Cache[name];
        }

        public static bool GetBool(string name)
        {
            var value = Get(name);
            return bool.TryParse(value, out var val) && val;
        }

        public static List<string> GetArray(string name)
        {
            return Get(name)?.Split(ArraySeparator, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())
                .ToList() ?? new List<string>();
        }

        public static void LoadEnvFiles()
        {
            var envFiles = new[] {".env", "../.env", ".local.env", "../.local.env"};
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var envFile in envFiles)
            {
                if (!File.Exists(envFile)) continue;

                Console.WriteLine("Loading .env file: " + envFile);
                DotEnv.Load(new DotEnvOptions(true, new[] {envFile}));
            }

            Console.ForegroundColor = oldColor;
        }

        private static readonly string[] ArraySeparator = {";"};
    }
}