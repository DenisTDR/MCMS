using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Data.Seeder;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Common.Translations.Languages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCMS.Common.Translations.Seed
{
    public class LanguagesSeeder : ISeeder
    {
        public async Task Seed(IServiceProvider serviceProvider, JArray seedData)
        {
            var langs = seedData.ToObject<List<LanguageEntity>>();
            var langsRepo = serviceProvider.Service<LanguagesRepository>();
            var logger = serviceProvider.Service<ILogger<LanguagesSeeder>>();
            langsRepo.SkipSaving = true;
            foreach (var lang in langs)
            {
                if (!await langsRepo.Any(l => l.Code == lang.Code))
                {
                    logger.LogInformation($"Adding language '{lang.Code}'...");
                    await langsRepo.Add(lang);
                }
            }

            await langsRepo.SaveChanges();
        }

        public async Task<JArray> BuildSeed(IServiceProvider serviceProvider)
        {
            var langs = await serviceProvider.Service<LanguagesRepository>().GetAll();
            var entries = langs.OrderBy(l => l.Code).Select(l => new LanguageEntity {Code = l.Code, Name = l.Name});
            return JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(entries,
                Utils.DefaultJsonSerializerSettings()));
        }

        public string SeedKey() => "languages";
    }
}