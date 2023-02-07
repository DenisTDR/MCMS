using System.Linq;
using System.Threading.Tasks;
using MCMS.Common.Translations.Languages;
using MCMS.Common.Translations.Translations.Item;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationsAdminApiController : CrudAdminApiController<TranslationEntity, TranslationFormModel,
        TranslationViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(t => t.Items)
                .ThenInclude(i => i.Language)
                .OrderBy(t => t.Slug));
        }

        [HttpGet]
        public async Task<ActionResult<TranslationFormModel>> GetInitialData()
        {
            var model = new TranslationFormModel();
            var langs = await Service<LanguagesRepository>().GetAll();
            model.Items = langs.OrderBy(lang => lang.Code).Select(lang => new ItemForTranslationFormModel
                {Language = new LanguageViewModel {Id = lang.Id}}).ToList();
            return model;
        }
    }
}