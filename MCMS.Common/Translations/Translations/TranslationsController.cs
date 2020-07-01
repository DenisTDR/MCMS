using System.Linq;
using MCMS.Controllers.Ui;
using MCMS.Display.ModelDisplay;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationsController : GenericModalAdminUiController<TranslationEntity, TranslationFormModel,
        TranslationViewModel,
        TranslationsAdminApiController>
    {
        protected override ModelDisplayTableConfig TableConfigForIndex()
        {
            var config = base.TableConfigForIndex();
            config.TableItemActions = config.TableItemActions.Where(tia => tia.Tag != "details").ToList();
            return config;
        }
    }
}