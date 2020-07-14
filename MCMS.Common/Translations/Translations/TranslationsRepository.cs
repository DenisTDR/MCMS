using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MCMS.Base.Helpers;
using MCMS.Base.Repositories;
using MCMS.Common.Translations.Languages;
using MCMS.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationsRepository : Repository<TranslationEntity>, ITranslationsRepository
    {
        public static string DefaultLanguage = Env.Get("DEFAULT_LANGUAGE") is { } str ? str : "en";

        private static readonly ConcurrentDictionary<string, Dictionary<string, TranslationCacheEntry>> Cache =
            new ConcurrentDictionary<string, Dictionary<string, TranslationCacheEntry>>();

        private readonly LanguagesRepository _langsRepo;
        private readonly ILogger<TranslationsRepository> _logger;

        public TranslationsRepository(BaseDbContext dbContext, LanguagesRepository langsRepo,
            ILogger<TranslationsRepository> logger) : base(dbContext)
        {
            _langsRepo = langsRepo;
            _logger = logger;
        }


        public async Task<Dictionary<string, TranslationCacheEntry>> GetForLanguage(string langCode = null)
        {
            langCode ??= DefaultLanguage;
            if (Cache.ContainsKey(langCode))
            {
                _logger.LogDebug("GetForLanguage hit cache.");
                return Cache[langCode];
            }

            _logger.LogDebug("GetForLanguage fetching from db.");

            var query = _queryable.Include(q => q.Items).ThenInclude(t => t.Language);
            var all = await query.ToListAsync();
            var langs = await _langsRepo.GetLanguagesCodes();
            var entries = all.ToDictionary(t => t.Slug, t => new TranslationCacheEntry
                {
                    IsRichText = t.IsRichText,
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

        public Task<string> Format(string slug, params object[] args)
        {
            return Format(slug, null, args);
        }

        public void ClearCache()
        {
            _logger.LogDebug("Clear cache.");
            Cache.Clear();
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

        public override Task<bool> Delete(Expression<Func<TranslationEntity, bool>> predicate)
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
                ReOrderByLang(one);
            }

            return one;
        }

        public override async Task<List<TranslationEntity>> GetAll(bool dontFetch = false)
        {
            var all = await base.GetAll(dontFetch);
            foreach (var translationEntity in all)
            {
                ReOrderByLang(translationEntity);
            }

            return all;
        }

        private void ReOrderByLang(TranslationEntity translationEntity)
        {
            if (translationEntity.Items == null)
            {
                return;
            }

            translationEntity.Items = translationEntity.Items.OrderBy(i => i?.Language.Code).ToList();
        }
    }
}