using System.Linq;
using System.Threading.Tasks;
using MCMS.Controllers.Ui;
using MCMS.Display.ModelDisplay;
using Microsoft.AspNetCore.Authorization;

namespace MCMS.Common.Translations.Translations.Item
{
    [Authorize(Roles = "Admin")]
    public class TranslationItemsController : GenericModalAdminUiController<TranslationItemEntity,
        TranslationItemFormModel,
        TranslationItemViewModel,
        TranslationItemsAdminApiController>
    {
        public override async Task<IndexPageConfig> GetIndexPageConfig()
        {
            var config = await base.GetIndexPageConfig();
            config.TableConfig.ItemActions = config.TableConfig.ItemActions.Where(tia => tia.Tag != "details").ToList();
            return config;
        }
    }
}