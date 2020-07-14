using System.Linq;
using System.Threading.Tasks;
using MCMS.Controllers.Ui;
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Authorization;

namespace MCMS.Common.Translations.Languages
{
    [Authorize(Roles = "Admin")]
    public class
        LanguagesController : GenericModalAdminUiController<LanguageEntity, LanguageFormModel, LanguageViewModel,
            LanguagesAdminApiController>
    {
        protected override async Task<ModelDisplayTableConfig> TableConfigForIndex()
        {
            var config = await base.TableConfigForIndex();
            config.TableItemActions = config.TableItemActions.Where(tia => tia.Tag != "details").ToList();
            return config;
        }
    }
}