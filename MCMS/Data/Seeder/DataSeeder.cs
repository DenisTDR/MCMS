using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MCMS.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCMS.Data.Seeder
{
    public class DataSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataSeeder> _logger;
        protected EntitySeeders _seeders;

        public DataSeeder(
            IServiceProvider serviceProvider,
            IOptions<EntitySeeders> seedersOptions,
            ILogger<DataSeeder> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _seeders = seedersOptions.Value;
        }

        public async Task SeedFromFile(string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Env.Get("SEED_FILE_PATH");
            }

            if (!File.Exists(fileName))
            {
                Utils.DieWith("Seed file '" + fileName + "' does not exist.");
            }

            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);
            var jsonStr = await sr.ReadToEndAsync();
            await Seed(jsonStr);
        }

        private async Task Seed(string jsonContent)
        {
            _logger.LogInformation("Seeding " + _seeders.Count + " seeders.");
            var seed = JsonConvert.DeserializeObject<Dictionary<string, JArray>>(jsonContent);
            foreach (var entitySeeder in _seeders)
            {
                _logger.LogInformation("Seeding from: " + entitySeeder.SeedKey());
                if (!seed.ContainsKey(entitySeeder.SeedKey().ToLower()))
                {
                    _logger.LogInformation("but not provided");
                    continue;
                }

                await entitySeeder.Seed(_serviceProvider, seed[entitySeeder.SeedKey()]);
            }
        }

        public async Task<Dictionary<string, JArray>> BuildSeed()
        {
            _logger.LogInformation("building seed");
            var seed = new Dictionary<string, JArray>();
            foreach (var entitySeeder in _seeders)
            {
                seed[entitySeeder.SeedKey()] =
                    await entitySeeder.BuildSeed(_serviceProvider);
            }

            return seed;
        }
    }
}