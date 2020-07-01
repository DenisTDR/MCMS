using System.Linq;
using MCMS.Controllers.Ui;
using MCMS.Display.ModelDisplay;

namespace MCMS.Common.Translations.Languages
{
    public class
        LanguagesController : GenericModalAdminUiController<LanguageEntity, LanguageFormModel, LanguageViewModel,
            LanguagesAdminApiController>
    {
        protected override ModelDisplayTableConfig TableConfigForIndex()
        {
            var config = base.TableConfigForIndex();
            config.TableItemActions = config.TableItemActions.Where(tia => tia.Tag != "details").ToList();
            return config;
        }
    }
}