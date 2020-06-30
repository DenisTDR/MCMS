using System.Linq;
using MCMS.Controllers.Ui;
using MCMS.Display.ModelDisplay;

namespace MCMS.Common.Translations.Translations
{
    public class TranslationsController : GenericUiModalController<TranslationEntity, TranslationFormModel,
        TranslationViewModel,
        TranslationsApiController>
    {
        protected override ModelDisplayTableConfig TableConfigForIndex()
        {
            var config = base.TableConfigForIndex();
            config.TableItemActions = config.TableItemActions.Where(tia => tia.Tag != "details").ToList();
            return config;
        }
    }
}