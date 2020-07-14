using System.Linq;
using MCMS.Controllers.Ui;
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Authorization;

namespace MCMS.Common.Translations.Translations.Item
{
    [Authorize(Roles = "Admin")]
    public class TranslationItemsController : GenericModalAdminUiController<TranslationItemEntity, TranslationItemFormModel,
        TranslationItemViewModel,
        TranslationItemsAdminApiController>
    {
        protected override ModelDisplayTableConfig TableConfigForIndex()
        {
            var config = base.TableConfigForIndex();
            config.TableItemActions = config.TableItemActions.Where(tia => tia.Tag != "details").ToList();
            return config;
        }
    }
}