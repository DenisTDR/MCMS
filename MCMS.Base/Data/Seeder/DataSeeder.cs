using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MCMS.Base.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCMS.Base.Data.Seeder
{
    public class DataSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataSeeder> _logger;
        private readonly EntitySeeders _seeders;
        private readonly SeedSources _sources;

        public DataSeeder(
            IServiceProvider serviceProvider,
            IOptions<EntitySeeders> seedersOptions,
            IOptions<SeedSources> seedSourcesOptions,
            ILogger<DataSeeder> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _sources = seedSourcesOptions.Value;
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
                Utils.DieWith($"Seed file '{fileName}' does not exist.");
            }

            _logger.LogInformation("Seeding from file: {FileName}", fileName);
            await using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            await SeedFromStream(fs);
        }

        public async Task SeedFromEmbeddedResource((Assembly, string) embeddedSource)
        {
            _logger.LogInformation("Seeding from embedded resource: {FileName} in Assembly: {Assembly}",
                embeddedSource.Item2, embeddedSource.Item1.FullName);
            await using var resource = embeddedSource.Item1.GetManifestResourceStream(
                $"{embeddedSource.Item1.GetName().Name}.{embeddedSource.Item2}");
            if (resource == null)
            {
                Utils.DieWith(
                    "Couldn't get/find a stream for a embedded resource for seed." +
                    $" Path: '{embeddedSource.Item2}', Assembly: '{embeddedSource.Item1.FullName}'");
            }

            await SeedFromStream(resource);
        }

        public async Task SeedFromStream([NotNull] Stream stream)
        {
            using var sr = new StreamReader(stream);
            var jsonStr = await sr.ReadToEndAsync();
            await Seed(jsonStr);
        }

        public async Task SeedFromProvidedSources()
        {
            foreach (var filePath in _sources.PhysicalSources)
            {
                await SeedFromFile(filePath);
            }

            foreach (var embeddedSource in _sources.EmbeddedSources)
            {
                await SeedFromEmbeddedResource(embeddedSource);
            }
        }

        private async Task Seed(string jsonContent)
        {
            var seed = JsonConvert.DeserializeObject<Dictionary<string, JArray>>(jsonContent);
            if (seed == null) return;

            foreach (var entitySeeder in _seeders)
            {
                var seedingKey = entitySeeder.SeedKey();
                if (!seed.ContainsKey(seedingKey))
                {
                    continue;
                }

                _logger.LogInformation("Seeding: {SeedingKey}", seedingKey);

                await entitySeeder.Seed(_serviceProvider, seed[seedingKey]);
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