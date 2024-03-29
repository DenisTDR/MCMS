using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Base.Data.Seeder;
using MCMS.Base.Extensions;
using MCMS.Base.Helpers;
using MCMS.Common.Translations.Languages;
using MCMS.Common.Translations.Translations;
using MCMS.Common.Translations.Translations.Item;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCMS.Common.Translations.Seed
{
    public class TranslationsSeeder : ISeeder
    {
        public async Task Seed(IServiceProvider serviceProvider, JArray seedData)
        {
            var seedTranslations = seedData.ToObject<List<TranslationSeedEntry>>();

            var logger = serviceProvider.Service<ILogger<TranslationsSeeder>>();
            var langs = await serviceProvider.Service<LanguagesRepository>().GetAll();
            var transRepo = serviceProvider.Service<TranslationsRepository>();
            var existingSlugs = await transRepo.Queryable.Select(t => t.Slug).ToListAsync();
            transRepo.SkipSaving = true;
            foreach (var translationSeedEntry in seedTranslations)
            {
                if (existingSlugs.Contains(translationSeedEntry.Slug))
                {
                    continue;
                }

                var toAdd = new TranslationEntity
                {
                    Slug = translationSeedEntry.Slug,
                    IsRichText = translationSeedEntry.IsRichText,
                    Tag = translationSeedEntry.Tag,
                    Items = translationSeedEntry.Items.Select(kvp => new TranslationItemEntity
                    {
                        Value = kvp.Value,
                        Language = langs.FirstOrDefault(l => l.Code == kvp.Key) ??
                                   throw new Exception("Can't seed translation for non existing language: " + kvp.Key)
                    }).ToList()
                };
                logger.LogInformation($"Adding translation '{toAdd.Slug}'...");
                await transRepo.Add(toAdd);
            }

            await transRepo.SaveChanges();
        }

        public async Task<JArray> BuildSeed(IServiceProvider serviceProvider)
        {
            var transRepo = serviceProvider.Service<TranslationsRepository>();

            var entries = await transRepo.BuildSeed();
            return JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(entries,
                Utils.DefaultJsonSerializerSettings()));
        }


        public string SeedKey() => "translations";
    }
}