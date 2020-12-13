using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCMS.Base.Helpers;
using MCMS.Base.Repositories;
using MCMS.Common.Translations.Languages;
using MCMS.Common.Translations.Seed;
using MCMS.Common.Translations.Translations.Item;
using MCMS.Data;
using MCMS.SwaggerFormly;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationsRepository : Repository<TranslationEntity>, ITranslationsRepository
    {
        public static string DefaultLanguage = Env.Get("DEFAULT_LANGUAGE") is { } str ? str : "en";

        private static readonly ConcurrentDictionary<string, Dictionary<string, TranslationCacheEntry>> Cache = new();

        private readonly LanguagesRepository _langsRepo;
        private readonly ILogger<TranslationsRepository> _logger;
        private readonly SwaggerConfigService _swaggerConfigService;

        public TranslationsRepository(
            BaseDbContext dbContext,
            LanguagesRepository langsRepo,
            ILogger<TranslationsRepository> logger,
            SwaggerConfigService swaggerConfigService) : base(dbContext)
        {
            _langsRepo = langsRepo;
            _logger = logger;
            _swaggerConfigService = swaggerConfigService;
        }


        public async Task<Dictionary<string, TranslationCacheEntry>> GetForLanguage(string langCode = null)
        {
            langCode ??= DefaultLanguage;
            if (Cache.ContainsKey(langCode))
            {
                // _logger.LogDebug("GetForLanguage hit cache.");
                return Cache[langCode];
            }

            _logger.LogDebug("GetForLanguage fetching from db.");

            var query = Queryable.Include(q => q.Items).ThenInclude(t => t.Language);
            var all = await query.ToListAsync();
            var langs = await _langsRepo.GetLanguagesCodes();
            var entries = all.ToDictionary(t => t.Slug, t => new TranslationCacheEntry
                {
                    IsRichText = t.IsRichText,
                    Tag = t.Tag,
                    Value = t.Items.OrderBy(i => i.Language.Code == langCode ? 0 : langs.IndexOf(i.Language.Code) + 1)
                        .FirstOrDefault()?.Value
                }
            );
            Cache[langCode] = entries;

            return Cache[langCode];
        }

        public async Task<TranslationCacheEntry> GetEntry(string slug, string lang = null)
        {
            lang ??= DefaultLanguage;
            var all = await GetForLanguage(lang);
            return all.ContainsKey(slug) ? all[slug] : null;
        }

        public async Task<string> GetValue(string slug, string lang = null)
        {
            return (await GetEntry(slug, lang))?.Value;
        }

        public async Task<string> GetValueOrSlug(string slug, string lang = null)
        {
            return (await GetEntry(slug, lang))?.Value ?? slug;
        }

        public async Task<string> Format(string slug, string lang = null, params object[] args)
        {
            var format = await GetValue(slug, lang);
            return string.Format(format, args);
        }

        public string Language => DefaultLanguage;


        public Task<string> Format(string slug, params object[] args)
        {
            return Format(slug, null, args);
        }

        public void ClearCache()
        {
            _logger.LogDebug("Clear cache.");
            Cache.Clear();
            _swaggerConfigService.ClearTranslationsFromCache();
        }

        public override Task<TranslationEntity> Add(TranslationEntity e)
        {
            foreach (var transItem in e.Items)
            {
                _langsRepo.Attach(transItem.Language);
            }

            ClearCache();
            return base.Add(e);
        }

        public override Task<TranslationEntity> Patch(string id, JsonPatchDocument<TranslationEntity> patchDoc,
            IAdapterFactory adapterFactory)
        {
            ClearCache();
            return base.Patch(id, patchDoc, adapterFactory);
        }

        public override Task<int> Delete(Expression<Func<TranslationEntity, bool>> predicate)
        {
            ClearCache();
            return base.Delete(predicate);
        }

        public override Task<bool> Delete(TranslationEntity e)
        {
            ClearCache();
            return base.Delete(e);
        }

        public override async Task<TranslationEntity> GetOne(string id)
        {
            var one = await base.GetOne(id);
            if (one?.Items != null)
            {
                await PatchLanguages(one);
            }

            return one;
        }

        public override async Task<List<TranslationEntity>> GetAll(
            Expression<Func<TranslationEntity, bool>> predicate = null)
        {
            var all = await base.GetAll(predicate);
            foreach (var translationEntity in all)
            {
                await PatchLanguages(translationEntity);
            }

            return all;
        }

        public async Task PatchLanguages(TranslationEntity translationEntity)
        {
            if (translationEntity?.Items == null)
            {
                return;
            }

            var langs = await _langsRepo.GetAll();
            var missingLangs = langs.Where(l => translationEntity.Items.All(i => i.Language.Code != l.Code));
            foreach (var languageEntity in missingLangs)
            {
                translationEntity.Items.Add(new TranslationItemEntity {Language = languageEntity});
            }


            translationEntity.Items = translationEntity.Items.OrderBy(i => i?.Language.Code).ToList();
        }

        public async Task<List<TranslationSeedEntry>> BuildSeed()
        {
            var anonEntries = await Queryable.Select(t => new
                {
                    t.Slug,
                    t.IsRichText,
                    t.Tag,
                    Items = t.Items.OrderBy(i => i.Language.Code)
                        .Select(i => new {lang = i.Language.Code, value = i.Value}).ToList()
                }
            ).ToListAsync();

            var entries = anonEntries.Select(ae => new TranslationSeedEntry
            {
                Slug = ae.Slug,
                IsRichText = ae.IsRichText,
                Tag = ae.Tag,
                Items = ae.Items.ToDictionary(i => i.lang, i => i.value),
            }).OrderBy(t => t.Slug).ToList();


            return entries;
        }

        public async Task<Dictionary<string, string>> GetAll(string langCode = null, string tag = null)
        {
            langCode ??= Language;
            var trans = await GetForLanguage(langCode);
            if (tag != null)
            {
                trans = trans.Where(kvp => kvp.Value.Tag == tag).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            return trans.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value);
        }
    }
}