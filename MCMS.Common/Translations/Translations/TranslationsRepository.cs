using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Common.Translations.Languages;
using MCMS.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationsRepository : Repository<TranslationEntity>
    {
        private readonly IRepository<LanguageEntity> _langsTrans;

        public TranslationsRepository(BaseDbContext dbContext, IRepository<LanguageEntity> langsTrans) : base(dbContext)
        {
            _langsTrans = langsTrans;
        }

        public override Task<TranslationEntity> Add(TranslationEntity e)
        {
            foreach (var transItem in e.Items)
            {
                _langsTrans.Attach(transItem.Language);
            }

            return base.Add(e);
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

        public override Task<TranslationEntity> Patch(string id, JsonPatchDocument<TranslationEntity> patchDoc, IAdapterFactory adapterFactory)
        {
            return base.Patch(id, patchDoc, adapterFactory);
        }

        public override Task<TranslationEntity> Patch(string id, JsonPatchDocument<TranslationEntity> patchDoc)
        {
            return base.Patch(id, patchDoc);
        }
    }
}