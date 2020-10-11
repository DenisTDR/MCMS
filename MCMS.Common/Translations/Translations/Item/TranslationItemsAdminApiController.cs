using System.Threading.Tasks;
using MCMS.Base.Extensions;
using MCMS.Common.Translations.Languages;
using MCMS.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

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

        protected override Task OnCreating(TranslationItemEntity e)
        {
            var repo = ServiceProvider.GetRepo<LanguageEntity>();
            repo.Attach(e.Language);
            return Task.CompletedTask;
        }
    }
}