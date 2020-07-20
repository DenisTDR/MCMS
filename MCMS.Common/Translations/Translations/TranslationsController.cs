using System.ComponentModel;
using System.Linq;
using MCMS.Controllers.Ui;
using MCMS.Display.Link;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Common.Translations.Translations
{
    [DisplayName("Translations")]
    public class TranslationsController : GenericModalAdminUiController<TranslationEntity, TranslationFormModel,
        TranslationViewModel,
        TranslationsAdminApiController>
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            Repo.ChainQueryable(q => q
                .Include(t => t.Items)
                .ThenInclude(i => i.Language)
                .OrderBy(t => t.Slug));
        }

        public override IActionResult Create()
        {
            var formConfig = FormParamsService.ForCreate();
            formConfig.GetUrl = new MLink("", typeof(TranslationsAdminApiController), "GetInitialData").BuildUrl(Url);
            ViewBag.FormParams = formConfig;
            return base.Create();
        }
    }
}