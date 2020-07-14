using System.Threading.Tasks;
using MCMS.Common.Translations.Languages;
using MCMS.Controllers.Api;
using MCMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MCMS.Common.Translations.Translations.Item
{
    [Authorize(Roles = "Admin")]
    public class
        TranslationItemsAdminApiController : GenericAdminApiController<TranslationItemEntity, TranslationItemFormModel,
            TranslationItemViewModel>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q.Include(t => t.Language));
        }

        protected override Task PatchBeforeSaveNew(TranslationItemEntity e)
        {
            var repo = ServiceProvider.GetService<IRepository<LanguageEntity>>();
            repo.Attach(e.Language);
            return Task.CompletedTask;
        }
    }
}