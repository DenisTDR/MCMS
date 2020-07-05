using MCMS.Common.Translations.Languages;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Common.Translations.Translations
{
    public class
        TranslationsAdminApiController : GenericAdminApiController<TranslationEntity, TranslationFormModel, TranslationViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Include(t => t.Language));
        }

        protected override void AttachFkProperties(TranslationEntity e)
        {
            var repo = ServiceProvider.GetService<IRepository<LanguageEntity>>();
            repo.Attach(e.Language);
        }
    }
}